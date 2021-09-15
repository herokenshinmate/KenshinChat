# KenshinChat

Simple Chat Application using an ASP Core Web API integrated with SignalR and JWT Authentication

API uses simple storage for users (very simple).
User login information is stored in a .json.

JWT Token is created on login or register, passed to the WPF which then uses it to establish a connection to the SignalR hub for chat
