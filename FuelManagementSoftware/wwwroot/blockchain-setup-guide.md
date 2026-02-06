# PetroChain™ Blockchain Setup Guide

You need to complete **3 registrations** below. At the end, you'll have **3 values** to paste into `appsettings.json`. The app will deploy the smart contract automatically on first run.

Total time: ~10 minutes.

---

## STEP 1: Create a Crypto Wallet (MetaMask)

MetaMask is a free browser extension that gives you a blockchain wallet (address + private key).

1. Open Chrome or Edge
2. Go to: **https://metamask.io/download/**
3. Click **"Install MetaMask for Chrome"**
4. Click **"Add to Chrome"** → **"Add Extension"**
5. MetaMask icon appears in your browser toolbar — click it
6. Click **"Create a new wallet"**
7. Agree to terms
8. Create a password (this is just for the browser extension, not blockchain)
9. **IMPORTANT**: MetaMask will show you a 12-word Secret Recovery Phrase — **write this down on paper and keep it safe**. You won't need it for this project, but never share it.
10. Confirm the recovery phrase
11. Your wallet is created!

### Switch to Sepolia Test Network

By default MetaMask is on "Ethereum Mainnet" (real money). We need the free test network.

1. Click the **network dropdown** at the top of MetaMask (it says "Ethereum Mainnet")
2. Click **"Show test networks"** (toggle it ON if needed)
3. Select **"Sepolia"**
4. MetaMask now shows "Sepolia" at the top and your balance is 0 SepoliaETH

### Get Your Wallet Address

1. In MetaMask, click on your account name at the top (it shows a shortened address like `0x1234...5678`)
2. Click the **copy icon** next to your address
3. **Save this** — this is your `WALLET ADDRESS`. It looks like: `0x742d35Cc6634C0532925a3b844Bc9e7595f2bD08`

### Get Your Private Key

1. In MetaMask, click the **three dots (⋮)** next to your account name
2. Click **"Account details"**
3. Click **"Show private key"**
4. Enter your MetaMask password
5. **Copy the private key** — it looks like: `4c0883a69102937d6231471b5dbb6204fe512961708279f15e3e0f83b1c38a1f`
6. **Save this** — this is your `PRIVATE KEY`

> ⚠️ NEVER share your private key with anyone. For this project it only controls test money (worthless), but it's good practice.

---

## STEP 2: Get a Free RPC Endpoint (Alchemy)

Alchemy gives you a free URL to connect to the Ethereum blockchain.

1. Go to: **https://dashboard.alchemy.com/signup**
2. Sign up with your email (or Google account)
3. On the welcome screen:
   - **Team name**: anything (e.g., "PetroChain")
   - Click **"Create App"** (or you'll land on the dashboard)
4. If not auto-created, click **"+ Create new app"**:
   - **Name**: `PetroChain`
   - **Chain**: select **Ethereum**
   - **Network**: select **Sepolia**
   - Click **"Create app"**
5. On your app dashboard, click **"API Key"** (or the app name)
6. You'll see your **API Key** and **HTTPS URL**. Copy the **HTTPS URL**.
   - It looks like: `https://eth-sepolia.g.alchemy.com/v2/your-api-key-here`
7. **Save this** — this is your `RPC URL`

---

## STEP 3: Get Free Test ETH (Sepolia Faucet)

You need a tiny amount of fake ETH to pay for transaction fees on the test network. It's completely free.

### Option A: Google Cloud Faucet (Recommended — No signup needed)

1. Go to: **https://cloud.google.com/application/web3/faucet/ethereum/sepolia**
2. Paste your **Wallet Address** from Step 1
3. Click **"Receive"**
4. Wait 10-30 seconds — you'll receive 0.05 SepoliaETH
5. Check MetaMask — your balance should update

### Option B: Alchemy Faucet (If Option A doesn't work)

1. Go to: **https://www.alchemy.com/faucets/ethereum-sepolia**
2. Sign in with your Alchemy account
3. Paste your **Wallet Address**
4. Click **"Send Me ETH"**
5. Wait 10-30 seconds

### Option C: QuickNode Faucet

1. Go to: **https://faucet.quicknode.com/ethereum/sepolia**
2. Connect your MetaMask wallet (click the button)
3. Click **"Claim"**

> You only need about 0.01 SepoliaETH. Any of the faucets above will give you enough for hundreds of test transactions.

---

## STEP 4: Paste Into Your App

Open the file `appsettings.json` in your project root and add the `Blockchain` section:

```json
{
  "ConnectionStrings": {
    "FuelManagementSoftwareConnection": "YOUR_EXISTING_CONNECTION_STRING"
  },
  "Blockchain": {
    "RpcUrl": "PASTE_YOUR_RPC_URL_HERE",
    "PrivateKey": "PASTE_YOUR_PRIVATE_KEY_HERE",
    "WalletAddress": "PASTE_YOUR_WALLET_ADDRESS_HERE",
    "ContractAddress": ""
  }
}
```

Replace the three values:

| Setting          | What to paste                          | Example                                                              |
|------------------|----------------------------------------|----------------------------------------------------------------------|
| `RpcUrl`         | HTTPS URL from Alchemy (Step 2)        | `https://eth-sepolia.g.alchemy.com/v2/abc123...`                     |
| `PrivateKey`     | Private key from MetaMask (Step 1)     | `4c0883a69102937d6231471b5dbb6204fe512961708279f15e3e0f83b1c38a1f`   |
| `WalletAddress`  | Wallet address from MetaMask (Step 1)  | `0x742d35Cc6634C0532925a3b844Bc9e7595f2bD08`                         |
| `ContractAddress`| **Leave empty** — the app will fill this automatically on first run | |

---

## STEP 5: Run the App

Once you've pasted the three values and started the app:

1. The app detects `ContractAddress` is empty
2. It automatically deploys the PetroChain smart contract to Sepolia
3. It saves the contract address back to the database (SystemConfiguration table)
4. From then on, every completed fuel transaction is recorded on the real Ethereum Sepolia blockchain
5. You can verify any transaction on **https://sepolia.etherscan.io** by searching the blockchain hash

---

## Verification Checklist

Before running the app, confirm:

- [ ] MetaMask installed and set to **Sepolia** network
- [ ] Wallet address copied (starts with `0x`)
- [ ] Private key copied (64 hex characters, no `0x` prefix)
- [ ] Alchemy app created on **Sepolia** network
- [ ] RPC URL copied (starts with `https://eth-sepolia.g.alchemy.com/v2/`)
- [ ] At least 0.01 SepoliaETH in your wallet (check MetaMask balance)
- [ ] All three values pasted into `appsettings.json`

---

## Cost

- **MetaMask**: Free
- **Alchemy**: Free (free tier allows 300M compute units/month — more than enough)
- **Sepolia ETH**: Free (test money, no real value)
- **Total cost: $0**

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| MetaMask doesn't show Sepolia | Click network dropdown → "Show test networks" toggle |
| Faucet says "insufficient funds" | Try a different faucet from the options above |
| Alchemy URL not working | Make sure you selected **Sepolia** (not Mainnet) when creating the app |
| App says "deployment failed" | Check you have SepoliaETH balance and private key is correct |
| Private key starts with `0x` | Remove the `0x` prefix — paste just the 64 hex characters |

