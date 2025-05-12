using System;
using Newtonsoft.Json;

[Serializable]
public class GPS {

    [JsonProperty("latitude")]
    public double Latitude;

    [JsonProperty("longitude")]
    public double Longitude;

    public override string ToString() {
        return $"{{latitude:{Latitude}, longitude:{Longitude}}}";
    }
}

[Serializable]
public class AircraftOrientation {

    [JsonProperty("pitch")]
    public double Pitch;

    [JsonProperty("roll")]
    public double Roll;

    [JsonProperty("yaw")]
    public double Yaw;

    [JsonProperty("compass")]
    public double Compass;
    
    public override string ToString() {
        return $"{{pitch:{Pitch}, roll:{Roll}, yaw:{Yaw}, compass:{Compass}}}";
    }
}

[Serializable]
public class AircraftVelocity {

    [JsonProperty("velocity_x")]
    public double X;

    [JsonProperty("velocity_y")]
    public double Y;

    [JsonProperty("velocity_z")]
    public double Z;

    public override string ToString() {
        return $"{{x:{X}, y:{Y}, z:{Z}}}";
    }
}

[Serializable]
public class GimbalOrientation {

    [JsonProperty("pitch")]
    public double Pitch;

    [JsonProperty("roll")]
    public double Roll;

    [JsonProperty("yaw")]
    public double Yaw;

    [JsonProperty("yaw_relative")]
    public double YawRelative;

    public override string ToString() {
        return $"{{pitch:{Pitch}, roll:{Roll}, yaw:{Yaw}, yaw_relative:{YawRelative}}}";
    }
}

[Serializable]
public class DroneFlightData : IJsonNotificationData {

    [JsonProperty("client_id")]
    public string ClientId;

    [JsonProperty("altitude")]
    public double Altitude;

    [JsonProperty("gps")]
    public GPS Gps;

    [JsonProperty("aircraft_orientation")]
    public AircraftOrientation AircraftOrientation;

    [JsonProperty("aircraft_velocity")]
    public AircraftVelocity AircraftVelocity;

    [JsonProperty("gimbal_orientation")]
    public GimbalOrientation GimbalOrientation;

    [JsonProperty("timestamp")]
    public string Timestamp;

    [JsonProperty("frame")]
    public string Frame;

    public override string ToString() {
        return $"{{client_id:{ClientId}, altitude:{Altitude}, gps:{Gps}, aircraft_orientation:{AircraftOrientation}, gimbal_orientation:{GimbalOrientation}, timestamp:{Timestamp}}}";
    }
}
