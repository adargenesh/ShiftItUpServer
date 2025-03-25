using System.ComponentModel.DataAnnotations;

namespace ShiftItUpServer.DTO
{
    public class WorkerShiftRequestDto
    {
        public int RequestId { get; set; }

        public int WeekNum { get; set; }

        public int Year { get; set; }

        public int WorkerId { get; set; }

        public string Remarks { get; set; } = null!;
        public WorkerShiftRequestDto() { }

        public WorkerShiftRequestDto(Models.WorkerShiftRequest modelWorker)
        { 
            this.RequestId = modelWorker.RequestId;
            this.WeekNum = modelWorker.WeekNum;  
            this.Year = modelWorker.Year;    
            this.WorkerId = modelWorker.WorkerId;    
            this.Remarks=modelWorker.Remarks;
        }
        public Models.WorkerShiftRequest GetModel()
        {

            Models.WorkerShiftRequest model = new Models.WorkerShiftRequest();
            model.RequestId = this.RequestId;   
            model.WeekNum = this.WeekNum;   
            model.Year = this.Year; 
            model.WorkerId = this.WorkerId; 
            model.Remarks = this.Remarks;
            return model;   

        }

     }
}
