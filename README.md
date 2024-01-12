# Umbraco Delivery API - member auth demo

This repo contains a demo of how protected content can be accessed for members using the Umbraco Delivery API.

The demo consists of a server and a client project - `src/Server` and `src/Client` respectively.

## The server

The server is an Umbraco 13 site, which means you'll need .NET 8 to run it. To start the server, open a terminal window in `src/Server` and run:

```bash
dotnet run
```

The Umbraco database is part of this repo, so the site should "just run" without any fuss.

The administrator login for Umbraco is:

- Username: admin@localhost
- Password: SuperSecret123

To facilitate member auth in the Delivery API, the `AuthorizationCodeFlow` is enabled in `appsettings.json`, and appropriate login and logout redirect URLs are defined for the client. A few more details about the auth flow can be found at the end of this README for anyone interested.

The Umbraco content features:
- A set of articles. Some of these are publicly available and some are protected.
- A login page. For the sake of this demo, a valid set of member credentials is hardcoded here.

By means of composition, the Umbraco site has:
- CORS enabled for the client.
- A custom path for the login page.
- Member auth support in the Swagger docs.

All composers can be found in the [`src/Server/Configuration`](/src/Server/Configuration) folder.

> [!TIP]
> For the sake this demo, the server implementation is kept as simple as possible. However, the Delivery API is capable of much more than "just" handling local member logins - for example, external login providers can also be used.
>
> More more details on the how's and why's of member auth in the Delivery API can be found in the [official documentation](https://docs.umbraco.com/umbraco-cms/reference/content-delivery-api/protected-content-in-the-delivery-api).

## The client

The client is a React app. The server must be running for the client to work. To start the client, open a terminal window in `src/Client` and run:

```bash
npm install
npm start
```

The client uses [AppAuth for JS](https://github.com/openid/AppAuth-JS) to handle the authorization flow complexity, and performs automatic discovery of the server OpenId configuration.

To keep this demo as simple as possible, the client consists of a single component called [`App`](/src/Client/src/App.js). This however has a few backdraws (which someone with better frontend skills than yours truly might be able to solve):
- The client flickers between authentication states when it reloads after a successful auth flow.
- The obtained access token is kept in memory, thus lost on reload.
- There seems to be no way to figure out when the authentication flow is truly finished. Incidentally, this is why there is a button to fetch content from the Delivery API manually rather than doing it automatically on load :smile:

In a real life scenario, the client would likely have a dedicated component to handle the auth flow "callback" (see below), and store the access token in local storage.

## The authorization flow

The Umbraco Delivery API uses the OpenId Connect flow _Authorization Code Flow + Proof Key of Code Exchange (PKCE)_ when performing member auth.

This is a complex flow that involves a fair bit of back-and-forth between the client and the server. A crucial point of this flow is that the client never knows about the member credentials - these should only be known to the server (or any third party authentication providers).

The flow goes something like this:

![Illustration of Authorization Code Flow + Proof Key of Code Exchange](/docs/auth-flow.png)

1. The client requests the `authorize` endpoint to initiate the flow. Among other things, this request must contain a valid return URL and a code challenge.
2. The server performs the authentication, usually by means of a login screen, but it could also be forwarding the request to a third party authentication provider. Subsequently it authorizes the "scope" of the initial client authorization request.
3. The server performs a callback request to the client return URL. This callback contains a code.
4. The client requests an access token from the `token` endpoint. Among other things, this request must contain the original redirect URL, the code from the server and a code verifier (which was initially used to generate the code challenge). These parameters are used by the server to validate the `token` request, and to ensure that it originates from the same client that initially sent the `authorize` request.
5. The `authorize` request yields an access token (among other things), which can be used as a bearer token in subsequent requests to the API.
