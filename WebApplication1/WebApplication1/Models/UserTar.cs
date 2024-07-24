using ProGaudi.MsgPack.Light;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [MsgPackArray]
    public class UserTar
    {
        public UserTar()
        {
        }

        [MsgPackArrayElement(0)]
        public int Id { get; set; }

        [MsgPackArrayElement(1)]
        public string FirstName { get; set; }

        [MsgPackArrayElement(2)]
        public string LastName { get; set; }

        [MsgPackArrayElement(3)]
        public string Email { get; set; }

        [MsgPackArrayElement(4)]
        public string Gender { get; set; }

        [MsgPackArrayElement(5)]
        public string Birthdate { get; set; }

        [MsgPackArrayElement(6)]
        public bool IsActive { get; set; }

        [MsgPackArrayElement(7)]
        public string IpAddress { get; set; }

        [MsgPackArrayElement(8)]
        public int Score { get; set; }

        [MsgPackArrayElement(9)]
        public string Created { get; set; }


        [MsgPackArrayElement(10)]
        public string UniqueId { get; set; }
    }
}
