<h1>A (very) simple sample to handle multiple languages in Azure Translator</h1>
<p>Here comes a sample for how to deal multiple languages in Azure Translator.</p>
<p>The short answer is that Azure Translator can handle multiple language documents for translation.</p>
<p>The long answer is that there are NOT reference sections dedicated to this purpose, nor are there samples provided on this topic.</p>
<p>So, I decided to conduct some research and come up with a simple sample for this purpose.</p>
<h2>Explain tehnical approaches in more details</h2>
There are three steps involved 
<ol>
<li>Decide a proper granular level for your chunks</li>
<p>Only one sentence in Japanese in a thousand page English book will not be properly detcted, if your chunk is generate at book, or chapter level.</p>  
<p>So, first, you have to decide how much granularity you want -- to make thing simpler, it can be sentence / paragraph / custom level.</p>
<p>For the first two, there are levels provided by some Azure internal services as well; the custom level, apparently, you have to provide your own way of generating  chunks.</p>
<p>For this sample, we are using "break sentence" service from Translator to set up granular level at sentence.</p>
<p>The processing time will be increased if your granular level become smaller; so there needs to be a balance bewteen accuracy and performance.  There are zero cost for your calls to break sentence or detect language, the cost only based on characters translated.</p>
<li>Detect languages inside one chunk</li>
<p>Translator can detect multiple languages, please see <a href="https://learn.microsoft.com/en-us/rest/api/cognitiveservices/translator/translator/detect?tabs=HTTP">a sample provided by Microsoft</a></p>
<p>However, they are coming back in different place, one will be selected as "Primary" lanaguge, and others are marked as "alternatives".</p>
<p>Below is a sample from the Microsoft link above, it has detected three language with 100% confidence, but language attribute shows only one, you need to go alternatives section for the rest two.</p> 
<code>
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
</code>
<p>There are several issues with language detection I have noticed regradless of granular level</p>
<ol>
  <li>It might NOT detect certain languages if their presence are minimal.</li>
  <li>It might NOT detect certain languages if their "root" are more / less the same, such as English / French / Germen, or Chinese / Japanese.</li>
  <li>It seems Translator never returning more than one language as alternatives, which is contradict to Microsoft documentations or samples.  I have asked <a href="https://learn.microsoft.com/en-us/answers/questions/1155532/azure-translator-can-only-detect-up-to-two-languag.html">a question in Azure community</a>.  It is highly appreciated that if you can share some insights with me!</li>
</ol>  
<li>Translate to the target language by specify the original language</li>
<p>After you have detected languages used in a chunk in the previous step, now you can send your chunk for translation by specify the original language that you want the Translator to take care one by one, instead of Translator only picks one primary language for translation purpose.  For example, a chunk is detect having French and Spanish, you want to first tell the translator to get all French taking care of, after that, you can ask Translator to take care of all Spanish.  Therefore, you get a translated chunk all in English!</p>
</ol>  
<h2>Setup and run codes</h2>
Here are some steps to set up and run codes in this repository
<ol>
<li>Download repository to your local</li>
<li>Open the solution with <a href="https://visualstudio.microsoft.com/vs/">VS2022 community version</a></li>
<li>Locate app.config file and change the lines below to your Azure Translator</li>
<code>
&lt;add key="translatorUrl" value="your_translator_url" /&gt;
&lt;add key="translatorKey" value="your_translator_key"/&gt;
&lt;add key="translatorRegion" value="your_translator_region"/&gt;
</code>
<li>Review the contents inside file multilang.txt, at the moment, if has French, Chinese, English and Japanese and you can change contents as your like</li>
<li>Target language for translation is hardcoded to English, you can change that in the following line.</li>
<code>
static string _toLanguage = "en";
</code>
<li>Compile and run, you should see the translated text in the console window</li>
</ol>
<h2>License</h2>
<p>Free software, absolutely NO warantty!  Use at your own risk.</p>  
