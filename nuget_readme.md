# Qoi.NetStandard ![GitHub](https://img.shields.io/github/license/RGgt/Qoi.NetStandard)
---
This is an implementation of QOI format (https://github.com/phoboslab/qoi) written in **.Net Standard 2.0** and therefore compatible with all modern versions of .Net, including **.Net Framework 4.6.1+**, **Net Core 2.0+**, **NET 5**, **NET 6**, and **Xamarin** (iOS 10.14+ and Android 8.0+).
 
 
---
 
# Sample usage

## Decoding a .qoi image

### From a file:
```csharp
// Decode a .qoi image from a file
byte[] colorBytes = Qoi.NetStandard.QoiEncoder.DecodeQoi(fileName, out Qoi.NetStandard.QoiHeader header);
// THAT'S ALL!

// Now you can use the decoded pixels to create an 
// image that can be displayed anywhere
Bitmap image = BitmapFromPixels(colorBytes, header);
pictureBox1.BackgroundImage = image;
```
Alternatively you can decode from streams or from bytes array.
### From a stream:
```csharp
// Decode a .qoi image from a stream
byte[] colorBytes = Qoi.NetStandard.QoiEncoder.DecodeQoi(stream, out Qoi.NetStandard.QoiHeader header);
...
```
### From a bytes array:
```csharp
// Decode a .qoi image from a bytes array
byte[] colorBytes = Qoi.NetStandard.QoiEncoder.DecodeQoi(qoiFileByes, out Qoi.NetStandard.QoiHeader header);
...
```



## Encoding a .qoi image
The main thing to take into account is that [decodedBytes] are the actual bytes of the image, so the (A)RGB information, not the bytes of the image file itself.
```csharp
byte[] qoiFileBytes = Qoi.NetStandard.QoiEncoder.EncodeToQoi(pngImage.Width,
    pngImage.Height,
    decodedBytes,
    hasAlpha,
    linearAlpha);
// THAT'S ALL!

// You can now write all these bytes as a .qoi file.
File.WriteAllBytes("myQoiImage.qoi", qoiFileBytes);
...
```
---
## List of compatible .Net implementations:

![GitHub](https://img.shields.io/badge/-Net_Standard_2.0+-blue) ![GitHub](https://img.shields.io/badge/-Net_Framework_4.6.1+-blue) ![GitHub](https://img.shields.io/badge/-Net_Core_2.0+-blue) ![GitHub](https://img.shields.io/badge/-NET_5.0+-blue) ![GitHub](https://img.shields.io/badge/-Mono_5.4+-blue) ![GitHub](https://img.shields.io/badge/-Xamarin.iOS_10.14+-blue) ![GitHub](https://img.shields.io/badge/-Xamarin.Android_8.0+-blue) ![GitHub](https://img.shields.io/badge/-Unity_2018+-blue) 
