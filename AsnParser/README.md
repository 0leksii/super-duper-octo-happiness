# Test task
There are two projects in the solution:
- AsnParser - containing the actual implementation
- AsnParserTests - containing some unit tests foe the main project

There are two main options in the [appsettings.json](AsnParser/appsettings.json):
- Directory - to configure the path to the directory where to watch for the files. Default value - ``/tmp/spooler``
- MongoDbUri - URI of the Mongo DB where to store results.

To run the solution - please setup the MongoDB to be available locally (e.g. as a Docker container) and set the correct URI.
Afterwards you will be able to run the solution locally.
Another option is to run inside the Docker. There is a [docker-compose-yml](docker-compose.yml) file that will setup both Mongo DB
and service itself. To add file to the folder that is watched simply run:
```
docker cp data.txt asn_parser:/tmp/spooler/
```

### Notes regarding the implementation
- There is quite a simple parsing - not taking into account any malformed data. I omitted the error handling in that part.
- Overall error handling is something that is not covered there in best way, even though there is a a pretty much global try/catch block it does not cover everything
- Overall the main point was given to the task itself: not to load whole file, monitor for file appearance and take care of RAM usage.