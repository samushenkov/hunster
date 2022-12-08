**Game stats tracker for Hunt: Showdown.** (MVP)

It parses *attributes.xml* file located at game folder and hosts an API service that provides realtime updates by using long polling approach.

**Configuration**

Configuration file *HunsterService.json* includes next settings:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Default": {
        "Url": "http://0.0.0.0:8088"
      }
    }
  },
  "GameFolderMatchTrackerOptions": {
    "GameProfileFolder": "C:/Program Files (x86)/Steam/steamapps/common/Hunt Showdown/user/profiles/default"
  }
}
```
**Swagger UI**

Swagger api is available at **http://localhost:8088/api/swagger**. 

**Static website host**
(NOT PRESENT IN THE CURRENT REPOSITORY)

Root endpoint can host static website located at ~/Web folder. This way it is possible to add http://localhost:8088 as browser source to OBS for realtime stats tracking.