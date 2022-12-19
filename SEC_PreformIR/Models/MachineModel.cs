namespace SEC_PreformIR.Models
{
    public class MachineModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ICollection<TemperatureModel> Temperatures { get; set; }
    }
}
