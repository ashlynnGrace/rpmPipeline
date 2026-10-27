namespace RpmPipeline;

public class PatientTelemetry
{
    public string PatientId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Core Vitals
    public int HeartRateBpm { get; set; }
    public double UrineOutputMlPerHour { get; set; }
    public int OxygenSaturationSpo2 { get; set; }
    public int SystolicBp { get; set; }
    public int DiastolicBp { get; set; }
}