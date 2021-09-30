# More badges

Just a few collection of badges which this world lacked.

#### API & Parameters

URL:
```
https://morebadges.tk/?badge=nugetdownloads&user=dotnetfoundation
```

`badge` is the required. All other parameters depend on the particular badge.

More info coming soon (this should be hosted somewhere).

## Badges

Here's a list of badges available.

### Nuget downloads per user/organization

`badge=nugetdownloads&user=WhiteBlackGoose`

Replace `microsoft` with the username:
```
https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dmicrosoft&query=Short&label=Nuget+downloads&logo=nuget
```

Example for Microsoft:

![Badge sample](https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dmicrosoft&query=Short&label=Nuget+downloads&color=lightblue)

![Badge sample](https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dmicrosoft&query=Short&label=Nuget+downloads&color=purple&style=flat-square)

![Badge sample](https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dmicrosoft&query=Short&label=Nuget+downloads&color=purple&style=flat-square&logo=nuget)

You can also use a format without shortening:

```
https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dmicrosoft&query=Normal&label=Nuget+downloads&color=purple&style=flat-square&logo=nuget
```

![Badge sample](https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dmicrosoft&query=Normal&label=Nuget+downloads&color=purple&style=flat-square&logo=nuget)

Another format:

```
https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dASC-Community&query=Short&label=NuGet+installs&logo=nuget&color=lightblue&style=social
```
![The number of downloads on nuget](https://img.shields.io/badge/dynamic/json?url=https%3A//morebadges.tk/%3fbadge=nugetdownloads%26user%3dASC-Community&query=Short&label=NuGet+installs&logo=nuget&color=lightblue&style=social)

## Hosting

.NET 6 SDK needed to build that thing. Also make sure to let the program listen the required port.
