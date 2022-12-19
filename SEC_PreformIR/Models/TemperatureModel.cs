using System.Diagnostics.CodeAnalysis;

namespace SEC_PreformIR.Models
{
    public class TemperatureModel
    {
        public int ID { get; set; }

        public string TemperatureValue { get; set; }

        public DateTime Timestamp { get; set; }



        // Relationships
        public int MachineID { get; set; }

    }
}
