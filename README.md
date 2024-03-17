# How To Use:

 - Ensure that Docker Desktop and Git are installed.
 - Then, open up your terminal.
- `git clone https://github.com/abogadotoast/webcrawler.git`
- Open up the folder labeled WebCrawler.
- Inside WebCrawler, you'll find WebCrawler.sln
- Once you're in the solution, press F5.
- This will spin up a docker container. 
	- If that doesn't work, try selecting a different option from the dropdown.
- Once loaded, you should see a Swagger endpoint on https://localhost:32782/swagger/index.html

# Testing
## Manual Testing
- Ensure that your Swagger endpoint is up.
### First Test Case
- Then put in the following keywords:
	- efiling
	- integration
- The urlToFindOnList is:
	- www.infotrack.com
- This should return a string "1, 2"
### Second Test Case
- Put in the following keywords:
	- infotrack
- The urlToFindOnList is:
	- www.infotrack.com
	- This should return a string of "1, 3, 6"
	- Occasionally this may vary as Google hates being scanned and tries obfuscating. 
		- If this is persistently not correct, please look at this equivalent test case but read in from an HTML file.
	
## Automated Testing:
There are two test projects.
 - WebCrawlerIntegrationTests 
 - WebCrawlerUnitTests
 - To use these, simply go to your Test Explorer in Visual Studio and Run All Tests
 - The WebCrawlerIntegrationTests mimic the manual testing done above and I would highly recommend checking those out. 
	 - There is both a file based integration test.
	 - There is also a web based integration test.
	 - The file based one is based on what I pulled down on my local machine from Google using the GET function with that user agent in Visual Studio 2022.

## FAQ
### What was your initial approach?
My initial approach was that this was going to be a trivial project. I thought I'd get together a couple Regexes, parse some text, and call it a night. This was more interesting than that. 
### What was the discovery process like?
Once I realized the Regexes weren't going to work, my next thought were trees.
Then the question became - what HTML node should I try and grab?
I initially went for the a href tags, but those kept getting Google images, so I changed strategies.
I then went for the cite tags, but those were getting duplicated.
Eventually I realized that if I can pivot on the h3 index, I can get the a href underneath.
So that's what I ended up building - a tree that recursively works its way down until it gets to where it needs to be.
Google doesn't make it easy! I have a new found appreciation for web scrapers now.
### How does this project keep to SOLID principles?
- Single-Responsibility Principle
	- The classes in this project have one clear responsibility.
- Open-Closed Principle
	- I use private methods to not expose implementation methods publicly. 
- Liskov Substitution Principle
	- A class may be replaced by its subclass.
	- It's not a concern here - everything is pretty flat as far as inheritance goes.
- Interface Segregration Principle
	- Almost all classes have interfaces.
	- The relationships between classes are dealt with abstract classes as often as possible.
	- I use interface types whenever possible.
- Dependency Inversion Principle
	- 	 I use inversion of control wherever possible, even in test suites.
	- 	Dependency injection is used EVERYWHERE.

### What are some things the next version of this WebCrawler could have?

- I could build fluent methods and predicate functions to make this more customizable.
- I could make the file writer more versatile.
- I could refactor the IHtmlNode as an abstract class that has its methods overridden, but there's no need to right now.
- I could probably refactor some IList to ICollection or IEnumerable.
- There's some tests where things repeat a bit - I could refactor those tests.
- Honestly, I worked pretty dang hard on this thing - I'm pretty proud of it.
