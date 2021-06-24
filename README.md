# Amicitia.github.io
This is a static set of HTML pages created dynamically using a C# program that also lives in this repository.  
Each page uses the same template, but the content is narrowed down by user input.  
The program iterates through user-submitted entries in a .tsv (tab-separated values) file to use as content.
![](https://i.imgur.com/MjH9Zve.png)

# User Features
## Browse by Entry Type
The header of the page is interactive. You can select a game or a type of entry (mods/tools/cheats/guides) to narrow down entries.  
Each page shows the total number of entries and pages per selection.  
Entries have a thumbnail, title, author, and upload date. There are fifteen entries per page by default.  
You can click on an entry to reveal more information, such as the description, links, and tags.  
Each entry has a link to the media (usually the thumbnail or a video) and a hyperlink to the specific entry.
## Browse by Tags & Authors
Entries with a similar subject, regardless of type, are grouped together by tags.  
You can click on a tag to view all entries that share it at once.  
You can view all entries submitted by a specific author by clicking on their username.
## Explore Other Resources
The navbar, sidebar, and footer are all closely intertwined with the rest of the Amicitia network of webpages for easy navigation to other handy modding info.
## Custom Themes
The color scheme changes based on game entry, unless you pick a preferred theme or create your own. Your selection is saved to your device's cookies.  
Additionally, you can toggle animated elements (waves/bubbles), keep track of bubbles popped, and toggle the popping noise.

# Why Static?
Github Pages provides a free and collaborative hosting solution, but only serves static pages.  
While a self-hosted website could allow dynamic page building, the functionality can be replicated to work on Github using the included program.  
Project Collaborators can submit their own changes to the page building code or database without using anything but GitHub and their editing tools of choice.  
Now the community can work together to host material without relying on a single webmaster.

# How to Contribute to the Database?
## Google Sheets
- Clone the repository using Git or Github Desktop.
- Upload the .tsv file in the "db" folder to your Google Drive account.
- Open it in Google Sheets, add/edit rows as needed.
- Download as .tsv when finished and replace the file in the "db" folder.
## Microsoft Excel
- Clone the repository using Git or Github Desktop.
- Open the .tsv file in the "db" folder.
- Add/edit rows as needed.
- Do NOT export directly, as this adds unwanted quotation marks.
- Enable the Developer Tab, go to it and choose Visual Basic.
- Use the included ExportTsvExcel.cls script to export as .tsv.
- Replace the file in the "db" folder.

# How to Generate New Pages?
- Open the solution (.sln) in Visual Studio (or your preferred IDE).
- Run the program to generate new pages.
- Commit changes to your own fork and open a pull request.

# Credits
## Gamebanana Webscraper
Code from [TekkaGB's AemulusModManager](https://github.com/TekkaGB/AemulusModManager) was used to achieve webscraping using [GBAPIv4](https://gamebanana.com/apiv4/).  
As such, this project is licensed under GPL-3.0.