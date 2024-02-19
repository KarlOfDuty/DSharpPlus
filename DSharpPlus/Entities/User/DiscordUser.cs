using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DSharpPlus.Net;
using DSharpPlus.Net.Abstractions;
using Newtonsoft.Json;

namespace DSharpPlus.Entities;

using Caching;

/// <summary>
/// Represents a Discord user.
/// </summary>
public class DiscordUser : SnowflakeObject, IEquatable<DiscordUser>
{
    internal DiscordUser() { }
    internal DiscordUser(TransportUser transport)
    {
        this.Id = transport.Id;
        this.Username = transport.Username;
        this.Discriminator = transport.Discriminator;
        this.GlobalName = transport.GlobalDisplayName;
        this.AvatarHash = transport.AvatarHash;
        this._bannerColor = transport.BannerColor;
        this.BannerHash = transport.BannerHash;
        this.IsBot = transport.IsBot;
        this.MfaEnabled = transport.MfaEnabled;
        this.Verified = transport.Verified;
        this.Email = transport.Email;
        this.PremiumType = transport.PremiumType;
        this.Locale = transport.Locale;
        this.Flags = transport.Flags;
        this.OAuthFlags = transport.OAuthFlags;
    }

    /// <summary>
    /// Gets this user's username.
    /// </summary>
    [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string Username { get; internal set; }

    /// <summary>
    /// Gets this user's global display name.
    /// </summary>
    /// <remarks>
    /// A global display name differs from a username in that it acts like a nickname, but is not specific to a single guild.
    /// Nicknames in servers however still take precedence over global names, which take precedence over usernames.
    /// </remarks>
    [JsonProperty("global_name", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string GlobalName { get; internal set; }

    /// <summary>
    /// Gets the user's 4-digit discriminator.
    /// </summary>
    /// <remarks>
    /// As of May 15th, 2023, Discord has begun phasing out discriminators in favor of handles (@username); this property will return "0" for migrated accounts.
    /// </remarks>
    [JsonProperty("discriminator", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string Discriminator { get; internal set; }

    [JsonIgnore]
    internal int DiscriminatorInt
        => int.Parse(this.Discriminator, NumberStyles.Integer, CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the user's banner color, if set. Mutually exclusive with <see cref="BannerHash"/>.
    /// </summary>
    public virtual DiscordColor? BannerColor
        => !this._bannerColor.HasValue ? null : new DiscordColor(this._bannerColor.Value);

    [JsonProperty("accent_color")]
    internal int? _bannerColor;

    /// <summary>
    /// Gets the user's banner url.
    /// </summary>
    [JsonIgnore]
    public string BannerUrl
        => string.IsNullOrEmpty(this.BannerHash) ? null : $"https://cdn.discordapp.com/banners/{this.Id}/{this.BannerHash}.{(this.BannerHash.StartsWith("a") ? "gif" : "png")}?size=4096";

    /// <summary>
    /// Gets the user's profile banner hash. Mutually exclusive with <see cref="BannerColor"/>.
    /// </summary>
    [JsonProperty("banner", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string BannerHash { get; internal set; }

    /// <summary>
    /// Gets the user's avatar hash.
    /// </summary>
    [JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string AvatarHash { get; internal set; }

    /// <summary>
    /// Gets the user's avatar URL.
    /// </summary>
    [JsonIgnore]
    public string AvatarUrl
        => !string.IsNullOrWhiteSpace(this.AvatarHash) ? (this.AvatarHash.StartsWith("a_") ? $"https://cdn.discordapp.com/avatars/{this.Id.ToString(CultureInfo.InvariantCulture)}/{this.AvatarHash}.gif?size=1024" : $"https://cdn.discordapp.com/avatars/{this.Id}/{this.AvatarHash}.png?size=1024") : this.DefaultAvatarUrl;

    /// <summary>
    /// Gets the URL of default avatar for this user.
    /// </summary>
    [JsonIgnore]
    public string DefaultAvatarUrl
        => $"https://cdn.discordapp.com/embed/avatars/{(this.DiscriminatorInt is 0 ? (this.Id >> 22) % 6 : (ulong)this.DiscriminatorInt % 5).ToString(CultureInfo.InvariantCulture)}.png?size=1024";

    /// <summary>
    /// Gets whether the user is a bot.
    /// </summary>
    [JsonProperty("bot", NullValueHandling = NullValueHandling.Ignore)]
    public virtual bool IsBot { get; internal set; }

    /// <summary>
    /// Gets whether the user has multi-factor authentication enabled.
    /// </summary>
    [JsonProperty("mfa_enabled", NullValueHandling = NullValueHandling.Ignore)]
    public virtual bool? MfaEnabled { get; internal set; }

    /// <summary>
    /// Gets whether the user is an official Discord system user.
    /// </summary>
    [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsSystem { get; internal set; }

    /// <summary>
    /// Gets whether the user is verified.
    /// <para>This is only present in OAuth.</para>
    /// </summary>
    [JsonProperty("verified", NullValueHandling = NullValueHandling.Ignore)]
    public virtual bool? Verified { get; internal set; }

    /// <summary>
    /// Gets the user's email address.
    /// <para>This is only present in OAuth.</para>
    /// </summary>
    [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string Email { get; internal set; }

    /// <summary>
    /// Gets the user's premium type.
    /// </summary>
    [JsonProperty("premium_type", NullValueHandling = NullValueHandling.Ignore)]
    public virtual PremiumType? PremiumType { get; internal set; }

    /// <summary>
    /// Gets the user's chosen language
    /// </summary>
    [JsonProperty("locale", NullValueHandling = NullValueHandling.Ignore)]
    public virtual string Locale { get; internal set; }

    /// <summary>
    /// Gets the user's flags for OAuth.
    /// </summary>
    [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
    public virtual UserFlags? OAuthFlags { get; internal set; }

    /// <summary>
    /// Gets the user's flags.
    /// </summary>
    [JsonProperty("public_flags", NullValueHandling = NullValueHandling.Ignore)]
    public virtual UserFlags? Flags { get; internal set; }

    /// <summary>
    /// Gets the user's mention string.
    /// </summary>
    [JsonIgnore]
    public string Mention
        => Formatter.Mention(this, this is DiscordMember);

    /// <summary>
    /// Gets whether this user is the Client which created this object.
    /// </summary>
    [JsonIgnore]
    public bool IsCurrent
        => this.Id == this.Discord.CurrentUser.Id;

    /// <summary>
    /// Unbans this user from a guild.
    /// </summary>
    /// <param name="guild">Guild to unban this user from.</param>
    /// <param name="reason">Reason for audit logs.</param>
    /// <returns></returns>
    /// <exception cref="Exceptions.UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.BanMembers"/> permission.</exception>
    /// <exception cref="Exceptions.NotFoundException">Thrown when the user does not exist.</exception>
    /// <exception cref="Exceptions.BadRequestException">Thrown when an invalid parameter was provided.</exception>
    /// <exception cref="Exceptions.ServerErrorException">Thrown when Discord is unable to process the request.</exception>
    public Task UnbanAsync(DiscordGuild guild, string reason = null)
        => guild.UnbanMemberAsync(this, reason);
    
    public ValueTask<DiscordPresence?> GetPresenceAsync() => this.Discord.Cache.TryGetUserPresenceAsync(this.Id);

    /// <summary>
    /// Gets the user's avatar URL, in requested format and size.
    /// </summary>
    /// <param name="imageFormat">The image format of the avatar to get.</param>
    /// <param name="imageSize">The maximum size of the avatar. Must be a power of two, minimum 16, maximum 4096.</param>
    /// <returns>The URL of the user's avatar.</returns>
    public string GetAvatarUrl(ImageFormat imageFormat, ushort imageSize = 1024)
    {
        if (imageFormat == ImageFormat.Unknown)
        {
            throw new ArgumentException("You must specify valid image format.", nameof(imageFormat));
        }

        // Makes sure the image size is in between Discord's allowed range.
        if (imageSize < 16 || imageSize > 4096)
        {
            throw new ArgumentOutOfRangeException("Image Size is not in between 16 and 4096: " + nameof(imageSize));
        }

        // Checks to see if the image size is not a power of two.
        if (!(imageSize is not 0 && (imageSize & (imageSize - 1)) is 0))
        {
            throw new ArgumentOutOfRangeException("Image size is not a power of two: " + nameof(imageSize));
        }

        // Get the string variants of the method parameters to use in the urls.
        string stringImageFormat = imageFormat switch
        {
            ImageFormat.Gif => "gif",
            ImageFormat.Jpeg => "jpg",
            ImageFormat.Png => "png",
            ImageFormat.WebP => "webp",
            ImageFormat.Auto => !string.IsNullOrWhiteSpace(this.AvatarHash) ? (this.AvatarHash.StartsWith("a_") ? "gif" : "png") : "png",
            _ => throw new ArgumentOutOfRangeException(nameof(imageFormat)),
        };
        string stringImageSize = imageSize.ToString(CultureInfo.InvariantCulture);

        // If the avatar hash is set, get their avatar. If it isn't set, grab the default avatar calculated from their discriminator.
        if (!string.IsNullOrWhiteSpace(this.AvatarHash))
        {
            string userId = this.Id.ToString(CultureInfo.InvariantCulture);
            return $"https://cdn.discordapp.com/{Endpoints.AVATARS}/{userId}/{this.AvatarHash}.{stringImageFormat}?size={stringImageSize}";
        }
        else
        {
            // https://discord.com/developers/docs/reference#image-formatting-cdn-endpoints: In the case of the Default User Avatar endpoint, the value for `user_discriminator` in the path should be the user's discriminator `modulo 5—Test#1337` would be `1337 % 5`, which evaluates to 2.
            string defaultAvatarType = (this.DiscriminatorInt % 5).ToString(CultureInfo.InvariantCulture);
            return $"https://cdn.discordapp.com/embed/{Endpoints.AVATARS}/{defaultAvatarType}.{stringImageFormat}?size={stringImageSize}";
        }
    }

    /// <summary>
    /// Returns a string representation of this user.
    /// </summary>
    /// <returns>String representation of this user.</returns>
    public override string ToString() => $"User {this.Id}; {this.Username}#{this.Discriminator}";

    /// <summary>
    /// Checks whether this <see cref="DiscordUser"/> is equal to another object.
    /// </summary>
    /// <param name="obj">Object to compare to.</param>
    /// <returns>Whether the object is equal to this <see cref="DiscordUser"/>.</returns>
    public override bool Equals(object obj) => this.Equals(obj as DiscordUser);

    /// <summary>
    /// Checks whether this <see cref="DiscordUser"/> is equal to another <see cref="DiscordUser"/>.
    /// </summary>
    /// <param name="e"><see cref="DiscordUser"/> to compare to.</param>
    /// <returns>Whether the <see cref="DiscordUser"/> is equal to this <see cref="DiscordUser"/>.</returns>
    public bool Equals(DiscordUser e) => e is null ? false : ReferenceEquals(this, e) ? true : this.Id == e.Id;

    /// <summary>
    /// Gets the hash code for this <see cref="DiscordUser"/>.
    /// </summary>
    /// <returns>The hash code for this <see cref="DiscordUser"/>.</returns>
    public override int GetHashCode() => this.Id.GetHashCode();

    /// <summary>
    /// Gets whether the two <see cref="DiscordUser"/> objects are equal.
    /// </summary>
    /// <param name="e1">First user to compare.</param>
    /// <param name="e2">Second user to compare.</param>
    /// <returns>Whether the two users are equal.</returns>
    public static bool operator ==(DiscordUser e1, DiscordUser e2)
    {
        object? o1 = e1 as object;
        object? o2 = e2 as object;

        return (o1 == null && o2 != null) || (o1 != null && o2 == null) ? false : o1 == null && o2 == null ? true : e1.Id == e2.Id;
    }

    /// <summary>
    /// Gets whether the two <see cref="DiscordUser"/> objects are not equal.
    /// </summary>
    /// <param name="e1">First user to compare.</param>
    /// <param name="e2">Second user to compare.</param>
    /// <returns>Whether the two users are not equal.</returns>
    public static bool operator !=(DiscordUser e1, DiscordUser e2)
        => !(e1 == e2);
}

internal class DiscordUserComparer : IEqualityComparer<DiscordUser>
{
    public bool Equals(DiscordUser x, DiscordUser y) => x.Equals(y);

    public int GetHashCode(DiscordUser obj) => obj.Id.GetHashCode();
}
