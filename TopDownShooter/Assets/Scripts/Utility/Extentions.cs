

namespace TopDownShooter.Utility.Extentions
{
    public static class VectorOperations
    {
        public static float Map(float value, float istart, float istop, float ostart, float ostop)
        {
            return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
        }
    }
}
