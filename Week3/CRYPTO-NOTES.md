# Cryptography & Security Notes

This document summarizes the core cryptographic and security concepts learned in Week 3.

---

## Day 1 — File Handling & Cryptographic Fundamentals

### FileStream and Chunked I/O
* **FileStream:** Operates directly on the OS filesystem level. Provides low-level read and write streams for binary data.
* **Chunking vs. Buffering:** Instead of calling helper methods like `File.ReadAllBytes(path)` (which reads the entire file into RAM, crashing applications on large files), streaming should always read files in designated chunk blocks (like 4 KB). This ensures that memory consumption remains tiny and constant, regardless of whether the file is 10 KB or 10 GB.
* **Resource Disposal:** Streams allocate handles outside the managed CLR memory scope. Always wrap stream usages in `using` blocks to prevent lock leaks.

### Base64 Encoding vs. SHA-256 Hashing vs. Encryption
* **Encoding (Base64):** Reversible representation of binary data in ASCII text characters. Secret-free and provides no security. Used for transport compatibility (e.g. sending files in JSON or emails).
* **Hashing (SHA-256):** One-way, irreversible mathematical digest. Converts arbitrary length inputs into a fixed-length string representation. Used to verify data integrity and compare passwords without storing the plain text.
* **Encryption (AES):** Reversible transformation of data using a secret key. Used to guarantee data confidentiality in storage or transit.

---

## Day 3 — Authenticated Encryption & Key Derivation (PBKDF2)

### PBKDF2 (Password-Based Key Derivation Function 2)
Allows deriving strong, random key bytes from weak human-memorized passwords.
* **Salt (16-byte random value):** Appended to the password before hashing. Prevents Rainbow Table attacks (pre-computed hash lists). Even if two users share the password "password123", their stored hashes will look completely different because of unique salts.
* **Iterations (>= 100,000):** Artificial computational delay. Slows down brute-force and dictionary attacks on leaked databases by forcing the processor to run the hash loop 100,000 times for every single password guess.

### AES-GCM (Authenticated Encryption)
* **Symmetric Key Size:** Modern standard uses a 256-bit key size (32 bytes).
* **Nonce (12-byte):** Number used once. Must never be reused under the same key.
* **Galois/Counter Mode (GCM):** Combines encryption with message authentication (AEAD). 
  * Unlike CBC (which only encrypts data and throws on padding errors), GCM generates a 16-byte **Authentication Tag** during encryption.
  * During decryption, GCM validates this tag. If a single bit of ciphertext or tag has been modified, GCM throws a `CryptographicException` immediately. This prevents padding-oracle attacks and malicious tampering.

---

## Day 5 — Asymmetric Encryption & Authentication Basics

### Asymmetric Encryption (RSA)
* **Asymmetric Keys:** Relies on a mathematically linked **Public Key** (used by anyone to encrypt payloads) and a **Private Key** (kept secret to decrypt payloads).
* **Size Limits:** RSA is mathematically slow and can only encrypt payloads smaller than its modulus size (e.g. 2048 bits minus padding overhead of ~66 bytes, limiting inputs to under 190 bytes).
* **Hybrid Encryption:** Used by TLS (HTTPS). 
  * Because RSA is too slow and limited for bulk data, the client generates a random **Symmetric Key** (AES key).
  * The client encrypts this small AES key using the server's **Public RSA Key** and sends it securely.
  * The server decrypts it using its **Private RSA Key**.
  * From then on, both parties stream bulk data using the fast **AES symmetric key**.

### Authentication vs. Authorization
* **Authentication (Who you are):** Verifying user identity credentials (e.g. verifying username and password matching stored PBKDF2 hashes during login).
* **Authorization (What you can do):** Enforcing access clearance levels for authenticated users (e.g., checking if the JWT token contains the role `Teacher` before allowing entry to write endpoints).
