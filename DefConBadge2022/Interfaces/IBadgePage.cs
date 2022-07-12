using System.Threading.Tasks;

namespace DefConBadge2022
{
    public interface IBadgePage
    {
        void Init(IHardwareConfig config);

        Task StartUpdating(IHardwareConfig config);

        Task StopUpdating();

        void Reset();

        void Left();
        void Right();
        void Up();
        void Down();
    }
}