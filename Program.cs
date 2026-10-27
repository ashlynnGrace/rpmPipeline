using RpmPipeline;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Endpoint to receive telemetry data from devices or mobile gateways
// Browser-friendly GET endpoint
app.MapGet("/api/telemetry", () => "RPM Telemetry API is live! Send an HTTP POST request with patient JSON data to submit vitals.");
app.MapPost("/api/telemetry", (PatientTelemetry telemetry) =>
{
    var alerts = new List<string>();

    // Rule 1: Check for Low Urine Output (Oliguria: < 30 mL/hr or < 0.5 mL/kg/hr)
    if (telemetry.UrineOutputMlPerHour > 0 && telemetry.UrineOutputMlPerHour < 30)
    {
        alerts.Add($"CRITICAL: Low urine output detected ({telemetry.UrineOutputMlPerHour} mL/hr). Possible acute renal impairment or dehydration.");
    }

    // Rule 2: Check for Abnormal Heart Rate
    if (telemetry.HeartRateBpm > 120)
    {
        alerts.Add($"WARNING: Tachycardia detected ({telemetry.HeartRateBpm} BPM).");
    }
    else if (telemetry.HeartRateBpm < 40 && telemetry.HeartRateBpm > 0)
    {
        alerts.Add($"WARNING: Bradycardia detected ({telemetry.HeartRateBpm} BPM).");
    }

    // Rule 3: Check Oxygen Levels
    if (telemetry.OxygenSaturationSpo2 > 0 && telemetry.OxygenSaturationSpo2 < 90)
    {
        alerts.Add($"CRITICAL: Hypoxia detected ({telemetry.OxygenSaturationSpo2}% SpO2).");
    }

    // Output alerts to the console (In production, push to nurses' dashboard or SMS system)
    if (alerts.Any())
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ALERT TRIGGERED - Patient {telemetry.PatientId}]");
        foreach (var alert in alerts)
        {
            Console.WriteLine($"  -> {alert}");
        }
        Console.ResetColor();
    }
    else
    {
        Console.WriteLine($"[INFO] Received normal vitals for Patient {telemetry.PatientId}");
    }

    return Results.Ok(new 
    { 
        Status = "Received", 
        PatientId = telemetry.PatientId,
        AlertCount = alerts.Count,
        Alerts = alerts
    });
});

app.Run();