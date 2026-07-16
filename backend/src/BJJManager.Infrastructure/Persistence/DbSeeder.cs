using BJJManager.Application.Common.Interfaces;
using BJJManager.Domain.Entities;
using BJJManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BJJManager.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(BjjManagerDbContext context, IPasswordHasher passwordHasher, CancellationToken cancellationToken = default)
    {
        if (await context.Users.AnyAsync(u => u.Name == "demo", cancellationToken))
        {
            return;
        }

        var demoUser = new User("demo", passwordHasher.Hash("Demo@123"));
        context.Users.Add(demoUser);

        context.TrainingSessions.AddRange(
            new TrainingSession(demoUser.Id, new DateOnly(2026, 7, 1), "Gracie Barra", SelfEvaluation.Good, "Worked on guard passing"),
            new TrainingSession(demoUser.Id, new DateOnly(2026, 7, 5), "Gracie Barra", SelfEvaluation.Average, "Need to improve armbar defense"),
            new TrainingSession(demoUser.Id, new DateOnly(2026, 7, 10), "Alliance", SelfEvaluation.Excellent, "Good conditioning today"));

        context.Techniques.AddRange(
            new Technique(
                demoUser.Id,
                "Armbar from Mount",
                "Mount",
                "Classic armbar finish from the mount position.",
                new[]
                {
                    "Break opponent posture.",
                    "Isolate one arm and control the wrist.",
                    "Swing the leg over the face and extend the hips to finish."
                }),
            new Technique(
                demoUser.Id,
                "Triangle Choke from Guard",
                "Closed Guard",
                "Submission using the legs to choke the opponent.",
                new[]
                {
                    "Control one arm and one sleeve.",
                    "Angle out and place the leg over the shoulder.",
                    "Lock the triangle and squeeze the knee down."
                }));

        await context.SaveChangesAsync(cancellationToken);
    }
}
