# JackHenryCodeExercise
Twitter API integration (coding example for Jack Henry)  
This effort reflects about 20 hours of work  
I spent a fair amount of time looking over the Twitter API documentation, but decided I wanted to focus more on the following requirement:  
"It’s very important that when the application receives a tweet it does not block statistics reporting while performing tweet processing. Twitter regularly sees 5700 tweets/second, so your app may likely receive 57 tweets/second, with higher burst rates. The app should process tweets as concurrently as possible to take advantage of available computing resources. While this system doesn’t need to handle the full tweet stream, you should think about how you could scale up your app to handle such a high volume of tweets."  
I did this by implementing the solution using an implementation of Azure Durable Functions (something I had not previously worked with). It was my belief that Azure Functions was the best choice as stated here by Microsoft:  
"https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview  
As requests increase, Azure Functions meets the demand with as many resources and function instances as necessary - but only while needed.  
As requests fall, any extra resources and application instances drop off automatically.  
Azure Functions provides as many or as few compute resources as needed to meet your application's demand."  
I chose a Durable Function over a standard (Azure) Function, as the orchestration allowed me to keep track of information coming back from event responses.  
I used a third-party plug in (SocialOpinionAPI) to communicate with the Twitter API. After reading more of the Twitter API documentation, it appears that this plug in may not accommodate all the functionality exposed by the Twitter API v2, But it appears to support everything requested in the exercise with the exception of emoji's (which I didn't focus a lot of effort on based on an email from my recruiter saying that part isn't critical).  
My implementation currently is started by a GET call to the Functions' API endpoint (StartTwitterOrchestrator). This could be changed to other trigger methods, but I didn't want to get too hung up on this part.  
I also want to note that I did all the development and debugging on my local machine using the Azure Storage Emulator, but I did not deploy the function to evaluate how it would work in the actual environment (I'm having issues with my Azure subscription). The output is just going to the logger, but since it can be pointed to Application Insights for analysis, I felt it was sufficient.  
  
  
Things I didn't get to fully implement:  
I added SOME tests, but I wanted to build them out (especially to demonstrate Mocking)  
I ran into an issue injecting the service and ended up instantiating the instance in the function call  
I used the Adapter pattern to wrap the 3rd party tool (SocialOpinionAPI). The event passed a type in the args that I used serialization to map to a shared type. To fix this, I am planning to implement a mapper (probably AutoMapper) in the wrapper class (probably in a convert method)  
I will likely also implement an in-memory instance of a database using EntityFramework (and surrounding tests)  
  
  
Regardless of the outcome of this, I really appreciate the challenge!

