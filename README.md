# azure-web-jobs-demo
Demo code for presentation on Azure WebJobs

**It will be difficult to build this project without reading all of this.**

I've attempted to set up some realistic, though 
simplistic, demonstrations of using WebJobs. The code is
mostly exploratory and I haven't used TDD much, though I 
have included some tests.

To get the application running you'll need to create
at least one Service Bus namespace. Because the example uses Topics, it must be at least Standard, not Basic. If
you wish to use Basic, you'll need to convert the jobs
that use Topics to use Service Bus Queues instead.

You will also need at least one storage account. This storage account can be used for all Azure storage needs for the application. The code assumes that you'll at
least be sharing them across all WebJobs, though you can
use separate ones for different purposes. I used the
same storage account for both dev and prod, but in
a real application you'd most likely use different
accounts for each environment. This is STRONGLY
recommended.

You will also need a way to send emails. I use PaperCut
for local delivery. For the example deployed in Azure
I set up a SendGrid free tier account and directly
connected to it via SMTP for mail delivery. YMMV.

Finally, all sensitive information is stored in a
set of configuration files that are kept in the 
ConfigurationFiles folder in the WebApp.  The other
projects include these same files as linked items
so I only have to maintain them in one place. All of
the non-library projects include SlowCheetah to
transform these configuration files when deployed
with the Release target.  You will also want to install
the SlowCheetah - XML Transforms Preview for 2015 to
get the tooling to allow you to add transforms to
all config files if you have additional build targets.

Because the configuration files contain sensitive
information (keys, passwords), they are not in source
control. I have included some example configuration
files with the sensitive information removed in
the ExampleConfigurationFiles.zip file. Extract
the zip into the WebApp folder, then edit them to 
add the appropriate dev and prod (Release) credentials 
and connection strings.

Good Luck! If you have questions, ping me on Twitter
@tvanfosson.
