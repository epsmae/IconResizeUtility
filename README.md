# Icon Resize Utility

## Intro
This repo contains a command line utility to create icon sets for Android and iOS development.
I often get icons from diffrent designers and usually the icons are not in the correct sizes, contain invalid characters or are like the material icons in seperate folders. This tool should assist for this annoying process.


|Description      |Link        |
|-----------------|------------|
|Sonarcloud       |[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=epsmae_IconResizeUtility&metric=alert_status)](https://sonarcloud.io/dashboard?id=epsmae_IconResizeUtility)|
|Build            |[![Build Status](https://my-tech-projects.visualstudio.com/Icon%20Resize%20Utility/_apis/build/status/Release%20Build?branchName=master)](https://my-tech-projects.visualstudio.com/Icon%20Resize%20Utility/_build/latest?definitionId=15&branchName=master)|
|Nuget            |[![Nuget](https://img.shields.io/nuget/v/IconResizeUtility.App)](https://www.nuget.org/packages/IconResizeUtility.App)|
|Nuget Prerelease |![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/IconResizeUtility.App)


## Roadmap

|Prio  |Description                                       |Status               |
|------|--------------------------------------------------|---------------------|
| 1    |Android image resizing                            | implemented         |
| 2    |iOS image resizing                                | implemented         |
| 3    |Add icons directly to Xamarin .sln                | implemented         |
| 4    |Icon tint color                                   | implemented         |
| 5    |Add opt out for rename to valid icon name         | planned             |
| 6    |Cache resized images to improve performance       | planned             |
| 7    |Move to res folders Android / iOS without resize  | planned             |
| 8    |Rename icon name                                  | planned             |
| 9    |Provide optional skip for existing                | planned             |



## Installation / Deinstallation
Use latest version

```bash
dotnet tool install --global IconResizeUtility.App
```

Use a specific version:

```bash
dotnet tool install --global IconResizeUtility.App --version 1.0.0-beta0014
```

Uninstall the tool:

```bash
dotnet tool uninstall --global IconResizeUtility.App
```

## Arguments

### resize

|Argument       |Description                                                           |Optional             |
|---------------|----------------------------------------------------------------------|---------------------|
| --type        |Droid or iOS                                                          | No                  |
| --srcFolder   |Folder where the icons are size > out size                            | No                  |
| --dstFolder   |Folder where the output will be written to                            | No                  |
| --iconSize    |Size of the icon (without scale factor)                               | Yes                 |
| --prefix      |Adds a prefix to the icon names                                       | Yes                 |
| --postfixSize |The postfix size can be skipt if only 1 size is created               | Yes                 |
| --csproj      |Full path to the Xamarin .csproj file where the icons should be added | Yes                 |
| --color       |Color #55FF5560 or "{\"red\":\"#FF0000\",\"green\":\"#00FF00\"}"      | Yes                 |

### resize

## Examples

### Resize with default sizes

Create resized images with the android default sizes.

See also [resize_default_droid.cmd](playground/resize_default_droid.cmd) and [resize_default_ios.cmd](playground/resize_default_ios.cmd).

```bash
IconResizeUtility resize --type droid --dstFolder out --srcFolder src --prefix ic_
Type: droid
Source folder: src
Destination folder: out
Prefix: ic_
Use icons sizes: 18, 20, 24, 36, 48
PostfixSize: True
```

The src folder looks like that

```bash
tree /F
    browser-user-help-message.png
    headphones-customer-support-human-1.png
    headphones-customer-support-human.png
    headphones-customer-support-question.png
    headphones-customer-support.png
    help-question-network.png
    help-wheel.png
    laptop-help-message.png
    question-help-circle.png
    question-help-message.png
    question-help-square.png
    user-question.png
```

And the result looks like that

```bash
tree /F
├───drawable-hdpi
│       ic_browser_user_help_message_18.png
│       ic_browser_user_help_message_20.png
│       ic_browser_user_help_message_24.png
│       ic_browser_user_help_message_36.png
│       ic_browser_user_help_message_48.png
|       ...
│
├───drawable-mdpi
│       ic_browser_user_help_message_18.png
|       ...
│
├───drawable-xhdpi
│       ...
├───drawable-xxhdpi
│       ...
└───drawable-xxxhdpi
        ...
```

### Resize with specific size

With a single size the postfix size can be set to false

See also [resize_specific_droid.cmd](playground/resize_specific_droid.cmd) and [resize_specific_ios.cmd](playground/resize_specific_ios.cmd).

```bash
IconResizeUtility resize --type droid --dstFolder out --srcFolder src --postfixSize false --iconSize 42
Type: droid
Source folder: src
Destination folder: out
Use icons sizes: 42
PostfixSize: False
```

The result looks like that

```bash
tree /F
├───drawable-hdpi
│       browser_user_help_message.png
│       headphones_customer_support.png
│       headphones_customer_support_human.png
│       ...
├───drawable-mdpi
│       browser_user_help_message.png
|       ...
├───drawable-xhdpi
│       ...
├───drawable-xxhdpi
│       ...
└───drawable-xxxhdpi
        ...
```

### Directly add to project file

Files can be directly added to Xamarin iOS and Android projects.

See also [resize_csproj_droid.cmd](playground/resize_csproj_droid.cmd) and [resize_csproj_ios.cmd](playground/resize_csproj_ios.cmd).

```bash
IconResizeUtility resize --type droid --dstFolder ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\Resources --srcFolder src --postfixSize false --iconSize 42 --csproj ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\ResizeUtility.App.Android.csproj
Type: droid
Source folder: src
Destination folder: ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\Resources
Use icons sizes: 42
PostfixSize: False
Csproj: ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\ResizeUtility.App.Android.csproj
```

### Tint color icons

Icons can be tinted.
See also [resize_csproj_droid_single_color.cmd](playground/resize_csproj_droid_single_color.cmd) and [resize_csproj_droid_multiple_color.cmd](playground/resize_csproj_droid_multiple_color.cmd).

```bash
IconResizeUtility resize --type droid --dstFolder ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\Resources --srcFolder src --postfixSize false --iconSize 42 --csproj ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\ResizeUtility.App.Android.csproj --color #FF0000
Type: droid
Source folder: src
Destination folder: ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\Resources
Use icons sizes: 42
PostfixSize: False
Csproj: ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\ResizeUtility.App.Android.csproj
Color: #FF0000
```

```bash
IconResizeUtility resize --type droid --dstFolder ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\Resources --srcFolder src --postfixSize false --iconSize 42 --csproj ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\ResizeUtility.App.Android.csproj --color "{\"red\":\"#FF0000\",\"green\":\"#00FF00\"}"
Type: droid
Source folder: src
Destination folder: ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\Resources
Use icons sizes: 42
PostfixSize: False
Csproj: ResizeUtility.App\ResizeUtility.App\ResizeUtility.App.Android\ResizeUtility.App.Android.csproj
Color: {"red":"#FF0000","green":"#00FF00"}
```
