using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class WaveSpawner
{
    // Tunables
    const float Gamma = 1.2f;    // how strongly Difficulty/Cost matters
    const float DupPenalty = 0.5f;  // k
    const float CostTilt   = 0.0f;  // α (0 = neutral; try 0.03 to gently prefer cheap)
    const int   BacktrackDepth = 2; // small repair search depth

    public static List<(string EnemyName, float TimeOffset)> GenerateWaveScheduled(
    IList<EnemyProfile> all, int waveBudget, float meanInterval, System.Random rng = null)
    {
        rng ??= new System.Random();
        var picks = new List<(string, float)>();
        var counts = new Dictionary<string, int>();
        var nextAvailable = new Dictionary<string, float>();
        float t = 0f;
        int remaining = waveBudget;

        bool Eligible(EnemyProfile e, int rem) =>
            e.Cost <= rem &&
            e.SpawnedThisWave < e.MaxPerWave &&
            t >= (nextAvailable.TryGetValue(e.EnemyName, out var na) ? na : 0f);

        int guard = 0;
        while (remaining >= 0 && ++guard < 5000)
        {
            var cands = new List<(EnemyProfile e, float w)>();
            foreach (var e in all)
            {
                if (!Eligible(e, remaining)) continue;
                int c = counts.TryGetValue(e.EnemyName, out var got) ? got : 0;
                float w = e.Weight * Mathf.Exp(-CostTilt * e.Cost) * (1f / (1f + DupPenalty * c));
                if (w > 0f) cands.Add((e, w));
            }
            if (cands.Count == 0)
            {
                // No one eligible now: advance t to the next time someone becomes eligible
                float next = float.PositiveInfinity;
                foreach (var e in all)
                {
                    if (e.Cost > remaining) continue;
                    if (e.SpawnedThisWave >= e.MaxPerWave) continue;

                    float ready = nextAvailable.TryGetValue(e.EnemyName, out var na) ? na : t;
                    if (ready > t && ready < next) next = ready;
                }

                if (float.IsPositiveInfinity(next))
                    break; // no one can ever become eligible again → done

                t = next;
                continue;
            }


            // roulette
            float total = cands.Sum(x => x.w);
            float r = (float)rng.NextDouble() * total;
            EnemyProfile chosen = null;
            foreach (var (e, w) in cands) { r -= w; if (r <= 0f) { chosen = e; break; } }
            chosen ??= cands[^1].e;

            // commit at time t
            picks.Add((chosen.EnemyName, t));
            counts[chosen.EnemyName] = counts.GetValueOrDefault(chosen.EnemyName) + 1;
            chosen.SpawnedThisWave++;
            remaining -= chosen.Cost;

            nextAvailable[chosen.EnemyName] = t + chosen.Cooldown;

            // advance virtual time by random inter-arrival
            float u = (float)rng.NextDouble();
            float dt = -Mathf.Log(1f - Mathf.Clamp01(u)) * meanInterval;
            t += dt;

            // stop if nothing fits by cost anymore
            if (!all.Any(e => e.Cost <= remaining && e.SpawnedThisWave < e.MaxPerWave))
                break;

        }

        return picks;
    }


    static bool GreedyFill(
        IList<EnemyProfile> all,
        List<string> picks,
        Dictionary<string, int> counts,
        float now,
        ref int remaining,
        Func<EnemyProfile,int,bool> eligible,
        System.Random rng)
    {
        int guard = 0; // safety
        while (remaining >= MinEligibleCost(all, remaining, now, eligible))
        {
            guard++; if (guard > 5000) break;

            // Build candidate list with dynamic weights
            var candidates = new List<(EnemyProfile e, float w)>();
            foreach (var e in all)
            {
                if (!eligible(e, remaining)) continue;
                int c = counts.TryGetValue(e.EnemyName, out var got) ? got : 0; ;
                float w = e.Weight
                        * Mathf.Exp(-CostTilt * e.Cost)
                        * (1f / (1f + DupPenalty * c));

                if (w > 0f) candidates.Add((e, w));
            }

            if (candidates.Count == 0) return false;

            // Roulette pick
            float total = candidates.Sum(t => t.w);
            float r = (float)rng.NextDouble() * total;
            EnemyProfile chosen = null;
            foreach (var (e, w) in candidates)
            {
                r -= w;
                if (r <= 0f) { chosen = e; break; }
            }
            chosen ??= candidates[^1].e;

            // Commit
            picks.Add(chosen.EnemyName);
            counts[chosen.EnemyName] = counts.GetValueOrDefault(chosen.EnemyName) + 1;
            chosen.SpawnedThisWave++;
            chosen.NextEligibleTime = now + chosen.Cooldown;
            remaining -= chosen.Cost;

            // If nothing else fits, break
            bool anyEligible = false;
            for (int i = 0; i < all.Count; i++)
            {
                if (eligible(all[i], remaining)) { anyEligible = true; break; }
            }
            if (!anyEligible) break;

        }
        return true;
    }

    static int MinEligibleCost(IList<EnemyProfile> all, int remaining, float now,
                               Func<EnemyProfile,int,bool> eligible)
    {
        int min = int.MaxValue;
        foreach (var e in all)
        {
            if (eligible(e, remaining)) min = Math.Min(min, e.Cost);
        }
        return min == int.MaxValue ? int.MaxValue : min;
    }

    static bool TryRepair(
        IList<EnemyProfile> all,
        List<string> picks,
        Dictionary<string, int> counts,
        float now,
        ref int remaining,
        int depth,
        System.Random rng)
    {
        if (remaining == 0 || depth <= 0) return remaining == 0;

        // Try to replace the last pick with a different choice that allows a better fill.
        for (int i = picks.Count - 1; i >= 0; --i)
        {
            string last = picks[i];
            var lastEnemy = all.First(e => e.EnemyName == last);

            // Undo last pick
            picks.RemoveAt(i);
            counts[last]--;
            if (counts[last] <= 0) counts.Remove(last);
            lastEnemy.SpawnedThisWave--;
            // cooldown rollback is approximated; if you need strict timing, track a stack of timestamps
            remaining += lastEnemy.Cost;

            // Try a different fill from here
            var snapshot = Snapshot(all);

            if (GreedyFill(all, picks, counts, now, ref remaining,
                (e, rem) => e.Cost <= rem && e.SpawnedThisWave < e.MaxPerWave && now >= e.NextEligibleTime,
                rng))
            {
                if (remaining == 0 || TryRepair(all, picks, counts, now, ref remaining, depth - 1, rng))
                    return true;
            }

            // Revert on failure and try earlier pick
            Restore(all, snapshot);
            picks.Insert(i, last);
            counts[last] = counts.GetValueOrDefault(last) + 1;
            lastEnemy.SpawnedThisWave++;
            remaining -= lastEnemy.Cost; // re-spend
        }

        return remaining == 0;
    }

    // Minimal state snapshot/restore for SpawnedThisWave & NextEligibleTime
    class EnemyState { public string name; public int spawned; public float next; }
    static List<EnemyState> Snapshot(IList<EnemyProfile> all) =>
        all.Select(e => new EnemyState { name = e.EnemyName, spawned = e.SpawnedThisWave, next = e.NextEligibleTime }).ToList();

    static void Restore(IList<EnemyProfile> all, List<EnemyState> snap)
    {
        var map = snap.ToDictionary(s => s.name, s => s);
        foreach (var e in all)
        {
            if (map.TryGetValue(e.EnemyName, out var s))
            {
                e.SpawnedThisWave = s.spawned;
                e.NextEligibleTime = s.next;
            }
        }
    }
}
