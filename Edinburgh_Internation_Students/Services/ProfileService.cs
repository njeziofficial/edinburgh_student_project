using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs;
using Edinburgh_Internation_Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Services;

public class ProfileService(ApplicationDbContext context) : IProfileService
{
    public async Task<(bool Success, ProfileResponse? Response, string ErrorMessage)> CreateProfileAsync(int userId, CreateProfileRequest request)
    {
        var user = await context.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return (false, null, "User not found");
        }

        if (user.Profile != null)
        {
            return (false, null, "Profile already exists for this user");
        }

        var profile = new Profile
        {
            UserId = userId,
            HomeCountry = request.HomeCountry,
            ShortBio = request.ShortBio,
            Campus = request.Campus,
            MajorFieldOfStudy = request.MajorFieldOfStudy,
            YearOfStudy = request.YearOfStudy,
            PreferredGroupSize = request.PreferredGroupSize,
            MatchingPreference = request.MatchingPreference,
            CreatedAt = DateTime.UtcNow
        };

        profile.SetInterests(request.Interests);
        profile.SetLanguages(request.Languages);

        await context.Profiles.AddAsync(profile);
        await context.SaveChangesAsync();

        var response = MapToProfileResponse(profile);
        return (true, response, string.Empty);
    }

    public async Task<(bool Success, ProfileResponse? Response, string ErrorMessage)> GetProfileByUserIdAsync(int userId)
    {
        var profile = await context.Profiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            return (false, null, "Profile not found");
        }

        var response = MapToProfileResponse(profile);
        return (true, response, string.Empty);
    }

    public async Task<(bool Success, ProfileResponse? Response, string ErrorMessage)> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var profile = await context.Profiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            return (false, null, "Profile not found");
        }

        if (!string.IsNullOrWhiteSpace(request.HomeCountry))
            profile.HomeCountry = request.HomeCountry;

        if (request.ShortBio != null)
            profile.ShortBio = request.ShortBio;

        if (!string.IsNullOrWhiteSpace(request.Campus))
            profile.Campus = request.Campus;

        if (!string.IsNullOrWhiteSpace(request.MajorFieldOfStudy))
            profile.MajorFieldOfStudy = request.MajorFieldOfStudy;

        if (request.YearOfStudy.HasValue)
            profile.YearOfStudy = request.YearOfStudy.Value;

        if (request.Interests != null && request.Interests.Count > 0)
            profile.SetInterests(request.Interests);

        if (request.PreferredGroupSize.HasValue)
            profile.PreferredGroupSize = request.PreferredGroupSize.Value;

        if (request.MatchingPreference.HasValue)
            profile.MatchingPreference = request.MatchingPreference.Value;

        if (request.Languages != null && request.Languages.Count > 0)
            profile.SetLanguages(request.Languages);

        profile.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var response = MapToProfileResponse(profile);
        return (true, response, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> DeleteProfileAsync(int userId)
    {
        var profile = await context.Profiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            return (false, "Profile not found");
        }

        context.Profiles.Remove(profile);
        await context.SaveChangesAsync();

        return (true, string.Empty);
    }

    private static ProfileResponse MapToProfileResponse(Profile profile)
    {
        return new ProfileResponse
        {
            Id = profile.Id,
            UserId = profile.UserId,
            HomeCountry = profile.HomeCountry,
            ShortBio = profile.ShortBio,
            Campus = profile.Campus,
            MajorFieldOfStudy = profile.MajorFieldOfStudy,
            YearOfStudy = profile.YearOfStudy.ToString(),
            Interests = profile.GetInterests(),
            PreferredGroupSize = GetGroupSizeDescription(profile.PreferredGroupSize),
            MatchingPreference = GetMatchingPreferenceDescription(profile.MatchingPreference),
            Languages = profile.GetLanguages(),
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }

    private static string GetGroupSizeDescription(PreferredGroupSize groupSize)
    {
        return groupSize switch
        {
            PreferredGroupSize.Small => "Small (3-4 members)",
            PreferredGroupSize.Medium => "Medium (5-6 members)",
            PreferredGroupSize.Large => "Large (7-8 members)",
            _ => groupSize.ToString()
        };
    }

    private static string GetMatchingPreferenceDescription(MatchingPreference preference)
    {
        return preference switch
        {
            MatchingPreference.SameCountry => "Same Country",
            MatchingPreference.DifferentCountries => "Different Countries",
            MatchingPreference.NoPreference => "No Preference",
            _ => preference.ToString()
        };
    }
}
