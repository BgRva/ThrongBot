Throng-of-Abots

ThrongBot is an application to run and manage multiple instances of the Abot Web 
Crawler (https://code.google.com/p/abot/).  Abot is an awesome crawler and the framework 
for customizing it is very intuitive and (the best part) not bloated.  Abot does the 
fetching, parsing, scheduling, and all the crawling.  ThrongBot provides an umbrella application
which manages multiple instances of Abot and allows spawning of new instances when new links
are found in a crawl.  Each instance of Abot is used to crawl a single domain (Abot excels at this).  
But for broad crawls, a new instance needs to be spawned for each new domain to be crawled.  ThrongBot
provides this glue and includes a common repository where the links to be crawled can be scheduled
from a common pool.  

The instances of Abot spawned in this application have been customized to meet some specific needs.  
Note that all of this customization fall within the documented means to customize Abot:  the 
configuration, overriding methods, and implementing interfaces, etc.  None of the core Abot classes
had to be modified.

The reason for implementing ThrongBot is to provide a means to gather research data on
certain types of online industries.  ThrongBot will be used for very broad and long
running crawls.  Previous incarnations (using other open source scrapers) had runs of 24 hrs
and produces about 220K links.  More will be posted about the project and its results.

ThrongBot also uses many other awesome open source software:  Log4Net, NInject, NUnit, and Moq.

More to come ...