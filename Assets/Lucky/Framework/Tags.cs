namespace Lucky.Framework
{
    public static class Tags
    {
        public static readonly BitTag PauseUpdate = new BitTag("pauseUpdate");
        public static readonly BitTag FrozenUpdate = new BitTag("frozenUpdate");
        public static readonly BitTag TransitionUpdate = new BitTag("transitionUpdate");
        // public static BitTag HUD = new BitTag("hud");
        // public static BitTag Persistent = new BitTag("persistent");
        // public static BitTag Global = new BitTag("global");
        // public static int AlwaysUpdate = PauseUpdate | FrozenUpdate | TransitionUpdate;
    }
}