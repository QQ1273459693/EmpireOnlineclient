
namespace Water2D
{

    public static class WaterShaderIdsMV
    {
        public static readonly string CameraPositionDelta = "_CameraPositionDelta";
        public static readonly string PositionDelta = "_PositionDelta";
        public static readonly string VelocityTexture = "_VelocityTexture";
        public static readonly string PreviousVelocityTexture = "_PreviousVelocityTexture";
        public static readonly string TemporaryVelocityTexture = "_TemporaryVelocityTexture";
        public static readonly string PixelScreenParams = "_PixelScreenParams";
        public static readonly string VelocitySimulationParams = "_VelocitySimulationParams";
    }

    public static class WaterShaderIdsREF
    {
        public static readonly string color = "_RFcolor";
        public static readonly string orgColor = "_RForgColor";
        public static readonly string alpha = "_RFalpha";
        public static readonly string reflectionsTexture = "_RFreflectionsTexture";
        public static readonly string reflectionsTexture2 = "_RFreflectionsTexture2";
    }

    public static class WaterShaderIdsSUR
    {
        public static readonly string surfaceTexture = "_SurfaceTexture";
    }

    public static class WaterShaderIdsOBS
    {
        public static readonly string OBStexture = "_OBStexture";
    }

}
