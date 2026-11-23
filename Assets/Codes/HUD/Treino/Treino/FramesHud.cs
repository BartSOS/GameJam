using Datas.Frames;

namespace HUD.Treino
{
public static class FramesHud
{
    public static bool TanqueVivo(Frame frame, int idTanque)
    {
        if (frame == null || frame.tanks == null) return false;
        var tanque = frame.tanks.Find(t => t.id == idTanque);
        return tanque != null && tanque.alive;
    }
}
}
