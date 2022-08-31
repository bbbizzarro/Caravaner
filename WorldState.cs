using Godot;
using Caravaner;
using System;

public class WorldState {
    public float Time {private set; get;}
    public const float DayLengthInMinutes = 24f * 60f;
    public const float DayToNightRatio = 0.3f;
    public const float TickLengthInMinutes = 10f;

    public float GetCurrentHour() {
        return Mathf.Floor(Time / 60f);
    }

    public float GetCurrentMinute() {
        float currHour = GetCurrentHour();
        return Mathf.Floor(((Time / 60f) % 1) * 60f);
    }

    public void AdvanceTime(float minutes) {
        Time = Utils.Mod(Time + minutes, DayLengthInMinutes);
    }

    public void SetToDay() {
        Time = DayLengthInMinutes * DayToNightRatio;
    }

    public void SetToNight() {
        Time = DayLengthInMinutes * (1f - DayToNightRatio);
    }

    public bool IsDay() {
        return !IsNight();
    }

    public bool IsNight() {
        return Time >= 0 && Time < DayLengthInMinutes * DayToNightRatio;
    }

    public string TimeToString() {
        return String.Format("{0}:{1}", GetCurrentHour().ToString("00"), GetCurrentMinute().ToString("00"));
    }

    public void AdvanceTimeByTicks(float amount) {
        AdvanceTime(amount * TickLengthInMinutes);
    }
}