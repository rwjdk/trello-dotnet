[Back to Attachment Features](TrelloClient#attachment-features)

Delete an Attachments on a card

## Signature (URL Attachment)
```cs
/// <summary>
/// Add an Attachment to a Card
/// </summary>
/// <param name="cardId">Id of the Card</param>
/// <param name="attachmentUrlLink">A Link Attachment</param>
/// <param name="cancellationToken">Cancellation Token</param>
/// <returns>The Created Attachment</returns>
public async Task<Attachment> AddAttachmentToCardAsync(string cardId, AttachmentUrlLink attachmentUrlLink, CancellationToken cancellationToken = default) {...}
```
## Signature (File Attachment)
```cs
/// <summary>
/// Add an Attachment to a Card
/// </summary>
/// <param name="cardId">Id of the Card</param>
/// <param name="attachmentUrlLink">A Link Attachment</param>
/// <param name="cancellationToken">Cancellation Token</param>
/// <returns>The Created Attachment</returns>
public async Task<Attachment> AddAttachmentToCardAsync(string cardId, AttachmentUrlLink attachmentUrlLink, CancellationToken cancellationToken = default) {...}
```

### Examples

```cs
var cardId = "63c939a5cea0cb006dc9e958";

//Add URL Link Attachment
Attachment urlAttachment = await _trelloClient.AddAttachmentToCardAsync(cardId, new AttachmentUrlLink("https://www.google.com", "Google Search"));

//Add File Attachment
Stream stream = File.OpenRead(@"D:\test.txt");
string filename = "test.txt";
string name = "My Optional Name for the File";
var fileAttachment = await _trelloClient.AddAttachmentToCardAsync(cardId, new AttachmentFileUpload(stream, filename, name));
```

![image](https://github.com/rwjdk/trello-dotnet/assets/7032102/b5e4fa20-6f35-4c4d-aeaa-9d2172c4b08e)