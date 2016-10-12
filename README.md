# ApiExamples

This repository contains example code for integrating with the SRS API. 

## Contents

In the [examples](./examples) folder you will find [C#](./examples/csharp), [JavaScript](./examples/javascript), & [VBA](./examples/vba) examples for integrating with the API.

## Environment

### C# #

1. You'll need to download and install [.Net 4.6](https://www.microsoft.com/en-us/download/details.aspx?id=53344) or greater
1. You'll need to download and install [LINQPad 5](https://www.linqpad.net/download.aspx) or [Visual Studio Community](https://www.visualstudio.com/vs/community/)
    - *`.linq` files contain C# code which can be used in a normal `.cs` file.*

### JS

1. Examples are written in [ES5](https://kangax.github.io/compat-table/es5/) syntax with [jQuery 3.1.0](https://blog.jquery.com/2016/07/07/jquery-3-1-0-released-no-more-silent-errors/) though it should be backwards compatable down to JQuery 1.8. 
1. Output is logged to the [console](https://developers.google.com/web/tools/chrome-devtools/console/)


### VBA

1. Enable the Developer Tab in your target application (Word, [Excel](https://www.techonthenet.com/excel/questions/developer_tab2013.php), Access, etc.)
1. You'll need to [add a reference](https://msdn.microsoft.com/en-us/library/office/gg264402.aspx) to:
    - `Microsoft XML, v6` (should work with any version)

## Configuration

1. You'll need a valid user account to access the API
1. You'll need the `basepath` to access the API
    - `http[s]://[yourSeverName]/SRSAPI/Generic`

## API Documentation

API documentation is available for your installed version of the API on your production server.

Visit `http[s]://[yourServerName]/SRSAPI/Generic/Help`

## Want to help?

- Check out our [CONTRIBUTING](./CONTRIBUTNG.md) guidelines
- Check out our [issues](/../../issues) list.