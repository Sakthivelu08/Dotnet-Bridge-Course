# Week 3 MCQ Practice & Edge Cases Preparation

This practice document contains tricky exam scenarios, edge cases, and twisted cohort questions related to Week 3 concepts (File Handling, Cryptography, Hashing, and JWT Authentication).

---

## Part 1: Multiple-Choice Questions (Self-Assessment)

### Q1: What is the main security risk of using AES in Electronic Codebook (ECB) mode?
1. It does not encrypt data, it only encodes it.
2. Identical plaintext blocks produce identical ciphertext blocks, preserving visual or structural patterns.
3. It requires an Initialization Vector (IV) which must be kept secret.
4. It is much slower than CBC and GCM modes.
* **Correct Answer: 2**
* **Explanation:** ECB encrypts each block independently using the same key without chaining. If two blocks have identical content (e.g. blank background pixels in a bmp image, or repetitive header structures in files), their resulting ciphertexts will be identical, allowing eavesdroppers to deduce patterns.

---

### Q2: What does `GC.SuppressFinalize(this)` do when called inside the `Dispose()` method of a file stream wrapper?
1. It forces the Garbage Collector to run immediately.
2. It deletes the temporary file from the disk.
3. It tells the GC that the object's unmanaged resources are already cleaned up, bypassing the finalizer (~Destructor) loop.
4. It prevents the object from being garbage collected forever.
* **Correct Answer: 3**
* **Explanation:** If `Dispose()` has cleared the unmanaged file handles, running the finalizer is redundant and wastes CPU resources. Suppressing it improves garbage collection efficiency by allowing the GC to reclaim the memory in a single cycle.

---

### Q3: Why is `CryptographicOperations.FixedTimeEquals` used instead of `SequenceEqual` or `==` for comparing signature digests?
1. It is mathematically faster and uses less CPU memory.
2. It prevents timing side-channel attacks by executing in constant time regardless of when a byte mismatch is found.
3. It automatically converts strings to byte arrays.
4. It uses SHA-256 to hash the arrays before comparing them.
* **Correct Answer: 2**
* **Explanation:** Standard comparison operators (`==`) exit early on the first mismatched byte, creating minor execution time differences. Attackers can measure these nanoseconds to guess the signature byte-by-byte (timing attack). `FixedTimeEquals` checks every single byte regardless of mismatch location, taking constant time.

---

### Q4: When using AES-GCM, what happens if an attacker modifies a single byte of ciphertext in transit?
1. The plaintext decrypts with garbage characters in the modified block, and no exception is thrown.
2. The decryption automatically corrects the corrupted byte using the parity bit.
3. Decryption fails and throws an `AuthenticationTagMismatchException` immediately, returning zero decrypted data.
4. The decryption works fine, but the key is corrupted.
* **Correct Answer: 3**
* **Explanation:** AES-GCM is an authenticated cipher (AEAD). It calculates a 16-byte authentication tag during encryption. If the ciphertext or tag is tampered with, the calculated tag won't match, causing GCM to throw an exception and reject the entire operation.

---

### Q5: What is the primary purpose of the "Salt" parameter in a password key derivation function like PBKDF2?
1. To make the password longer so it meets length requirements.
2. To encrypt the password using asymmetric encryption.
3. To generate a unique hash for identical passwords, neutralizing pre-computed lookup tables (Rainbow Tables).
4. To make the database query execute faster.
* **Correct Answer: 3**
* **Explanation:** Without a salt, identical passwords yield identical hashes. If a database is leaked, an attacker can compare the hashes against pre-computed lists (Rainbow Tables) of common passwords. A unique salt makes every stored hash unique, forcing the attacker to calculate hashes individually.

---

### Q6: Why is asymmetric encryption (like RSA) not used to encrypt large files (e.g. 100 MB videos)?
1. RSA requires a different public key for every chunk of data.
2. RSA is computationally slow and has a strict payload size limit smaller than the key modulus size.
3. RSA ciphertexts are 10 times larger than the original files.
4. RSA is insecure for static storage.
* **Correct Answer: 2**
* **Explanation:** RSA performs heavy modular exponentiation math, making it thousands of times slower than AES. Furthermore, a 2048-bit RSA key can only encrypt payloads smaller than 244 bytes (depending on padding), making it impossible to encrypt large streams directly.

---

### Q7: If a user modifies their role claim from "Student" to "Teacher" inside their local JWT token storage, what happens when they call a secured API endpoint?
1. The API validates it because JWT is client-controlled.
2. The API accepts the claim but logs a warning.
3. The API rejects the request because the cryptographic signature becomes invalid.
4. The server automatically regenerates the token.
* **Correct Answer: 3**
* **Explanation:** A JWT signature is calculated by hashing the Header + Payload using the server's private secret key. If a user modifies the payload claims locally, the signature will no longer match the recalculated hash, and the server's middleware will reject the token with `401 Unauthorized` or `403 Forbidden`.

---

## Part 2: Critical Edge Cases & Troubleshooting

### 1. The Stream Disposal Order Trap
When streaming encrypted files, you chain streams together:
$$\text{FileStream (Target)} \longleftarrow \text{CryptoStream} \longleftarrow \text{StreamWriter}$$

* **Edge Case:** If you manually close/dispose the inner `FileStream` before calling `CryptoStream.FlushFinalBlock()`, the encryptor will crash with an `ObjectDisposedException` because it tries to write the final padding block to a closed stream.
* **Rule:** Always let the outermost wrapper dispose first (which propagates down), or use `using` blocks where disposal runs from inside-out automatically.

### 2. PBKDF2 Iteration Counts
* **Edge Case:** Using low iteration counts (e.g., 1,000 iterations) is a major audit failure.
* **Rule:** Current security standards require at least 100,000 to 600,000 iterations (using SHA-256 or SHA-512) to ensure brute-force attacks are slow enough on modern CPU/GPU mining cards.

### 3. Asymmetric Hybrid Key Exchange
* **Flow:** 
  1. Client sends request to Server.
  2. Server sends its **Public RSA Key** (inside an SSL certificate).
  3. Client generates a random **Symmetric Key** (AES key) and encrypts it with the server's **Public RSA Key**.
  4. Server decrypts the AES key using its private **Private RSA Key**.
  5. Both stream the session data using the shared **AES Key**.
* **Cohort Question:** Why does TLS use this hybrid approach?
  * *Answer:* To get the best of both worlds: asymmetric RSA for secure identity validation and initial key sharing (requires no pre-shared secret), and symmetric AES for high-speed bulk data streaming.
