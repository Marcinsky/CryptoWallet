import React, { useState, useEffect } from "react";
import axios from "axios";  // Importujemy axios

const CryptoWallet = () => {
    const [coinName, setCoinName] = useState("");
    const [entryPrice, setEntryPrice] = useState("");
    const [depositUSD, setDepositUSD] = useState("");
    const [wallet, setWallet] = useState([]);

    useEffect(() => {
        fetchWallet();
    }, []);

    // Fetching wallet data from the backend using Axios
    const fetchWallet = async () => {
        try {
            const response = await axios.get("http://localhost:7052/api/wallets");
            setWallet(response.data);
        } catch (error) {
            console.error("Error fetching wallet:", error);
        }
    };

    // Function for adding a coin using Axios
    const handleAddCoin = async (e) => {
        e.preventDefault();
        if (!coinName || entryPrice <= 0 || depositUSD <= 0) {
            alert("Please enter valid data!");
            return;
        }

        const newCoin = {
            CoinName: coinName,
            EntryPriceUSD: parseFloat(entryPrice),
            AmountOwned: depositUSD / entryPrice,
            CurrentPriceUSD: 0, // Placeholder for future price update
        };

        try {
            const response = await axios.post("http://localhost:7052/api/wallets", newCoin, {
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (response.status === 201) {
                fetchWallet(); // Refresh the wallet data
                setCoinName("");
                setEntryPrice("");
                setDepositUSD("");
            }
        } catch (error) {
            console.error("Error adding coin:", error);
        }
    };


    // Function for deleting a coin using Axios
    const handleDeleteCoin = async (id) => {
        try {
            const response = await axios.delete(`http://localhost:7052/api/wallets/${id}`);

            if (response.status === 200) {
                fetchWallet(); // Refresh the wallet data
            }
        } catch (error) {
            console.error("Error deleting coin:", error);
        }
    };

    return (
        <div className="container mt-4">
            <h2 className="text-center">Crypto Wallet</h2>
            <form onSubmit={handleAddCoin}>
                {/* Form content */}
                <div className="mb-3">
                    <label htmlFor="coinName" className="form-label">Coin Name</label>
                    <input
                        type="text"
                        id="coinName"
                        className="form-control"
                        value={coinName}
                        onChange={(e) => setCoinName(e.target.value)}
                    />
                </div>
                <div className="mb-3">
                    <label htmlFor="entryPrice" className="form-label">Entry Price (USD)</label>
                    <input
                        type="number"
                        id="entryPrice"
                        className="form-control"
                        value={entryPrice}
                        onChange={(e) => setEntryPrice(e.target.value)}
                    />
                </div>
                <div className="mb-3">
                    <label htmlFor="depositUSD" className="form-label">Deposit (USD)</label>
                    <input
                        type="number"
                        id="depositUSD"
                        className="form-control"
                        value={depositUSD}
                        onChange={(e) => setDepositUSD(e.target.value)}
                    />
                </div>
                <button type="submit" className="btn btn-primary">Add Coin</button>
            </form>

            {/* Table displaying wallet */}
            {wallet.length > 0 && (
                <table className="table mt-4">
                    <thead>
                        <tr>
                            <th>Coin Name</th>
                            <th>Entry Price</th>
                            <th>Amount Owned</th>
                            <th>Current Price</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {wallet.map((coin) => (
                            <tr key={coin.id}>
                                <td>{coin.coinName}</td>
                                <td>{coin.entryPriceUSD}</td>
                                <td>{coin.amountOwned}</td>
                                <td>{coin.currentPriceUSD}</td>
                                <td>
                                    <button
                                        className="btn btn-danger"
                                        onClick={() => handleDeleteCoin(coin.id)}
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};

export default CryptoWallet;
