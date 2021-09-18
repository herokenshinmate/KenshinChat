# KenshinChat

Simple Chat Application using an ASP Core Web API integrated with SignalR and JWT Authentication

API uses simple storage for users (very simple).
User login information is stored in a .json.
No personal information is required to create a user. 

 **NOTE:** *It is worth using a throw away and simple password during testing and never use a password you use for anything else.*

JWT Token is created on login or register, passed to the WPF which then uses it to establish a connection to the SignalR hub for chat


### Current Features:
  - [x] JWT Token Authorization & Authentication
  - [x] Profile Pictures (Profile pictures are stored as byte[])
  - [x] Online / Offline tracking
  - [x] Typing tracking
  - [x] Message History

#### Future Features:
  - [ ] Private Channels (create your own channels, invite users to channels)
  - [ ] Send and Receive Images
  - [ ] Send and Receive Gifs

## Technical

## EndPoints

### [POST] api/User/login
##### Authenticated: 
False
##### PostBody
```
{
  "userId": 0,
  "profilePicture": "string",
  "username": "string",
  "password": "string"
}
```
##### Response
```
{
  "UserId": 0,
  "ProfilePicture": "string",
  "username": "string",
  "AccessToken": "string"
}
```

### [POST] api/User/register
##### Authenticated: 
False
##### PostBody
```
{
  "userId": 0,
  "profilePicture": "string",
  "username": "string",
  "password": "string"
}
```
##### Response
```
{
  "UserId": 0,
  "ProfilePicture": "string",
  "username": "string",
  "AccessToken": "string"
}
```

### [POST] api/User/GetProfilePicture
##### Authenticated: 
True
##### PostBody
```
{
  "AccessToken": "string",
  "userId": 0
}
```
##### Response
```
{
  "ProfilePicture" = "string"
}
```
