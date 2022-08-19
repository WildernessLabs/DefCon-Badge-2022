using System.Threading.Tasks;

namespace DefConBadge2022
{
    public interface IBadgePage
    {
        void Init(IHardwareConfig config);

        void StartUpdating(IHardwareConfig config);

        void StopUpdating();

        void Reset();

        void Left();
        void Right();
        void Up();
        void Down();
    }
}