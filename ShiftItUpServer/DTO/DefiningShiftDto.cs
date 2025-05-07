namespace ShiftItUpServer.DTO
{
    public class DefiningShiftDto
    {
        public int DefiningShiftId { get; set; }

        public int IdStore { get; set; }

        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public int NumEmployees { get; set; }

        public DefiningShiftDto() { }

        public DefiningShiftDto(Models.DefiningShift modelDefiningShift)
        {
            this.DefiningShiftId = modelDefiningShift.DefiningShiftId;
            this.IdStore = modelDefiningShift.IdStore;
            this.DayOfWeek = modelDefiningShift.DayOfWeek;
            this.StartTime = modelDefiningShift.StartTime;
            this.EndTime = modelDefiningShift.EndTime;
            this.NumEmployees = modelDefiningShift.NumEmployees;
        }

        public Models.DefiningShift GetModel()
        {
            Models.DefiningShift model = new Models.DefiningShift();
            model.DefiningShiftId = this.DefiningShiftId;
            model.IdStore = this.IdStore;
            model.DayOfWeek = this.DayOfWeek;
            model.StartTime = this.StartTime;
            model.EndTime = this.EndTime;
            model.NumEmployees = this.NumEmployees;

            return model;
        }
    }
}
