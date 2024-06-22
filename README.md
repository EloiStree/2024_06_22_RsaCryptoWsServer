
# 2024_06_22_RsaCryptoWsServer

Crypto and RSA can be a pain outside of C#.

- Tried to do Ethereum sign verification in Python... Failed
- Tried to convert PEM to XML in most languages... Failed
- Tried to implement RSA in Unity3D... Not supported
- Tried to do sign verification with RSA in Rust... Failed a lot

Tried to do all that in C#:
- Just download NuGet and use it

Conclusion after months of failing with different languages:
Why not just create a C# WebSocket server with all the necessary utilities callable from other languages?
