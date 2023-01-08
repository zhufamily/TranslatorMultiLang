# A (very) simple sample to hanele multiple languages in Azure Translator
Here comes a sample for dealing multiple languages in Azure Translator.\
The short answer is that Azure Translator can handle multiple language documents for translation.\
The long answer is that there are NOT reference sections dedicated to this purpose, nor are there samples provided on this topic.\
So, I decided to conduct some research and come up with a simple sample for this purpose.
## Explain tehnical approaches in more details
There are three steps involved 
1. Decide a proper granular level for your chunks\
Only one sentence in Japanese in a thousand page English book will not be properly detcted, if your chunk is generate at book, or chapter level.  
So, first, you have to decide how much granularity you want -- to make thing simpler, it can be sentence / paragraph / custom level.
For the first two, there are levels provided by some Azure internal services as well; the custom level, apparently, you have to provide your own way of dealing with chunks.\
For this sample, we are using "break sentence" service from Translator to set up granular level at sentence.
2. Detect languages inside one chunk\
Translator can detect multiple languages, please see [the sample from Microsoft](https://learn.microsoft.com/en-us/rest/api/cognitiveservices/translator/translator/detect?tabs=HTTP)\
However, they are coming back in different place, one will be selected as "Primary" lanaguge, and others are marked as "alternatives".\
Below is a sample from Microsoft, it has detected three language with 100% confidence, but language attribute shows only one, you need to go alternative section for the rest two. 
```
[
  {
    "language": "en",
    "score": 1,
    "isTranslationSupported": true,
    "isTransliterationSupported": false,
    "alternatives": [
      {
        "language": "fil",
        "score": 1,
        "isTranslationSupported": true,
        "isTransliterationSupported": false
      },
      {
        "language": "nb",
        "score": 1,
        "isTranslationSupported": true,
        "isTransliterationSupported": false
      }
    ]
  }
]
```
