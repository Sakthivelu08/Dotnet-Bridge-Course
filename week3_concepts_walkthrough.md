# Week 3 Concepts Walkthrough: Cryptography & Authentication

This document provides a comprehensive guide to the file streaming, cryptography, hashing, and role-based authentication concepts implemented in Week 3.

---

## 1. File Handling & Chunked I/O

### The Concept
When dealing with file operations in C#, the key priority is **memory efficiency**. Helper methods like `File.ReadAllBytes` read the entire file into RAM at once. For a 1 GB file, this allocates 1 GB of memory, which can crash your application due to out-of-memory errors on production servers.

Instead, we use a streaming approach where we allocate a fixed-size buffer (e.g., 4 KB) and read/write the file chunk-by-chunk in a loop. Memory usage remains constant (4 KB) regardless of whether the file is 10 KB or 10 GB.

### Real-Life Analogy
Think of moving water from a pool to a garden. Reading all bytes is like trying to lift the entire pool at once (impossible and crashes the system). Chunked streaming is like using a bucket (the buffer) to transfer water bucket-by-bucket.

### Production Example (Chunked File Copy)
```csharp
using System;
using System.IO;

public static class FileStreamer
{
    public static void CopyFile(string src, string dest)
    {
        byte[] buffer = new byte[4096]; // 4 KB buffer
        using (var fsIn = new FileStream(src, FileMode.Open, FileAccess.Read))
        using (var fsOut = new FileStream(dest, FileMode.Create, FileAccess.Write))
        {
            int bytesRead;
            while ((bytesRead = fsIn.Read(buffer, 0, buffer.Length)) > 0)
            {
                fsOut.Write(buffer, 0, bytesRead);
            }
        }
    }
}
```

---

## 2. Hashing vs. Symmetric Encryption vs. Encoding

| Mechanism | Purpose | Key | Reversibility | Output Size |
| :--- | :--- | :--- | :--- | :--- |
| **Encoding (Base64)** | Mappings for transport compatibility. | None. | Fully reversible. | Variable (grows by ~33%). |
| **Hashing (SHA-256)** | Verifying data integrity. | None. | One-way (Irreversible). | Fixed (32 bytes / 256 bits). |
| **Symmetric Encryption (AES)** | Ensuring data confidentiality. | Secret Key. | Reversible (with key). | Matches plaintext + padding block boundaries. |

### The Integrity Gap (CBC Mode)
Symmetric encryption ciphers (like AES-CBC) provide **confidentiality** (hiding data) but **not integrity** (detecting modifications). If an attacker alters a byte of ciphertext in transit, AES-CBC will decrypt it into garbage data without throwing an error. A `CryptographicException` is only thrown if the final garbage block fails PKCS7 padding format checks.

---

## 3. Authenticated Encryption (AES-GCM) & Key Derivation (PBKDF2)

### PBKDF2 (Password-Based Key Derivation Function 2)
Humans choose weak passwords (e.g. `Password123`). We run these through PBKDF2 to derive a strong, mathematically random 256-bit key.
* **Salt:** A random array of bytes appended to the password before hashing. Prevents **Rainbow Table attacks** (precompiled hash lookups).
* **Iterations:** The number of times the hashing algorithm loops (e.g., 100,000). This adds computational delay, making brute-force guesses too slow to execute.

### AES-GCM (AEAD Cipher)
AES-GCM is an **Authenticated Encryption with Associated Data (AEAD)** cipher. During encryption, it outputs both the ciphertext and a 16-byte **Authentication Tag**.
* During decryption, it verifies this tag. If a single bit of the ciphertext or tag has been tampered with, the validation fails, and it throws an `AuthenticationTagMismatchException` immediately, preventing garbage data leakage.

### Real-Life Analogy
AES-CBC is like mailing a sealed letter. Someone could open it, modify the contents, seal it back up, and you wouldn't know.  
AES-GCM is like mailing a sealed letter inside a locked steel box. If anyone tampers with the lock, it refuses to open, alerting you instantly.

---

## 4. Keyed Integrity: HMAC & Timing Side-Channels

### Hash vs. HMAC (Hash-Based Message Authentication Code)
* **Hash (SHA-256):** Tells you *what* changed, but not *who* did it. If an attacker changes a file, they can simply recalculate the SHA-256 hash and replace it.
* **HMAC:** Keyed hash. Proves **authenticity**. Only parties who possess the secret HMAC key can calculate a valid digest, proving the message came from an authorized source.

### Constant-Time Comparisons
Standard string comparisons (`==` or `SequenceEqual`) return `false` as soon as they find a mismatch. An attacker can measure how many microseconds the check took to guess where the mismatch occurred, enabling them to reconstruct signatures byte-by-byte (a **Timing Attack**).

We use `CryptographicOperations.FixedTimeEquals` to compare byte arrays. It always compares every byte, taking the exact same amount of time regardless of when a mismatch occurs.

---

## 5. Asymmetric Key Limits (RSA) & Hybrid Encryption

### RSA Modulus Limits
Asymmetric encryption (RSA) relies on public and private key pairs. However, RSA is mathematically slow and can only encrypt payloads smaller than its key size (minus padding overhead). For a 2048-bit RSA key, the maximum payload is under 190 bytes.

### Hybrid Encryption (TLS / HTTPS)
To secure bulk internet traffic, we combine the security of RSA with the speed of AES:
1. The client generates a random symmetric **AES Key** (32 bytes).
2. The client encrypts this small key using the server's public **RSA Key** and sends it.
3. The server decrypts the AES key using its private **RSA Key**.
4. Both parties communicate using fast **AES symmetric encryption** for the remainder of the session.

---

## 6. Token-Based Authentication & Role-Based Authorization

### Authentication vs. Authorization
* **Authentication:** Verifies identity (e.g. verifying password hashes on login).
* **Authorization:** Verifies access permissions (e.g. checking user roles).

### Token (JWT) Composition
A JSON Web Token (JWT) is split into three parts separated by periods (`.`):
1. **Header:** Identifies the signature algorithm (e.g. HMAC-SHA256).
2. **Payload:** Contains custom user data claims (e.g. username, role, expiration timestamp).
3. **Signature:** Calculated by hashing the Header + Payload using the server's private secret key. This prevents clients from modifying claims (like elevating their role from `Student` to `Teacher`) because any modification invalidates the signature.

---

## ⚠️ Twisted Cohort Questions (Q&A Prep)

1. **Why does AES-GCM require a Nonce instead of an IV? What happens if you reuse a Nonce?**
   * *Answer:* An IV (Initialization Vector) only needs to be random. A Nonce (Number used once) must be strictly unique per encryption run. If you reuse the same key and the same Nonce to encrypt two different files, an attacker can XOR the two ciphertexts together, completely canceling out the encryption key stream and exposing the plaintext immediately.
2. **Does a JWT encrypt user payload claims by default?**
   * *Answer:* **No.** A standard JWT is only Base64-encoded, not encrypted. Anyone can decode a JWT and view your claims (username, roles, email). The signature only prevents *tampering*, it does not hide the data. Sensitive data (like user passwords or addresses) should never be placed in a standard JWT.
3. **Why do we use SHA-256 for file integrity but PBKDF2 for password storage?**
   * *Answer:* SHA-256 is designed to be **fast**. We want to calculate the hash of a 1 GB file as fast as possible. However, if a password database is leaked, an attacker can use high-speed GPU rigs to guess millions of SHA-256 passwords per second. PBKDF2 is designed to be **deliberately slow** (using iteration loops), making mass brute-force attacks too slow and expensive to perform.
