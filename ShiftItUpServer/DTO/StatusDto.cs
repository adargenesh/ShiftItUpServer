namespace ShiftItUpServer.DTO
{
    public class StatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;


        public StatusDto()
        {

        }


        public StatusDto(Models.Status modelStatus)
        {
            this.Id = modelStatus.Id;
            this.Name = modelStatus.Name;

        }

        public Models.Status GetModel()
        {
            Models.Status model = new Models.Status();
            model.Id = this.Id;
            model.Name = this.Name; 

            return model;
        }
    }
}
