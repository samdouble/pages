# Panels

This is a small application to export a comic book in PDF format from an XML configuration and a set of images.

## Technologies & Languages

- C# 11
- .NET 7
- iText 8
- GitHub Actions
- GitHub Releases

## Development

### Running locally

```
dotnet run --project Panels --framework net8.0 -- -c ~/Desktop/github/proj/bd1/BD0/bd.xml -i ~/Desktop/github/proj/bd1/BD0/images
```

### Testing

```
dotnet test
```

### Releasing a New Version

1. Change the version number in the *csproj* file.
2. Merge to the `master` branch.
3. dotnet-releaser on CircleCI will create a new GitHub Release.

## How to Use

### Command-line arguments
