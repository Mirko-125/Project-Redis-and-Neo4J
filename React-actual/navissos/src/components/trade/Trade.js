import React, { useEffect, useState } from 'react';
import './Trade.css';
import '../../styling/CrudContainer.css';


function Trade() {
    const [receiverGold, setReceiverGold] = useState(0);
    const [requesterGold, setRequesterGold] = useState(0);
    const [receiverName, setReceiverName] = useState(0);
    const [requesterName, setRequesterName] = useState(0);
    const [receiverItemNames, setReceiverItemNames] = useState([]);
    const [requesterItemNames, setRequesterItemNames] = useState([]);
    const [tradeID, setTradeID] = useState(0);
    const [marketGold, setMarketGold] = useState(0);
    const [playerGold, setPlayerGold] = useState(0);
    const [date, setDate] = useState("string");
    const [playerID, setPlayerID] = useState(0);
    const [marketplaceID, setMarketplaceID] = useState(0);
    const [playerItemNames, setPlayerItemNames] = useState(["string"]);
    const [marketItemNames, setMarketItemNames] = useState(["string"]);
    const [selectedMarketTrade, setMarketTrade] = useState([]);
    
    const handleCreateTrade = () => {
        const tradeData = {
            receiverGold: receiverGold,
            requesterGold: requesterGold,
            receiverName,
            requesterName,
            receiverItemNames: receiverItemNames,
            requesterItemNames: requesterItemNames
        };

        fetch('http://localhost:5236/api/Trade', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(tradeData)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };
    const handleFinalizeTrade = () => {
        const tradeData = {
            tradeID: tradeID,
            receiverGold: receiverGold,
            requesterGold: requesterGold
        };

        fetch('http://localhost:5236/api/Trade/Finalize', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(tradeData)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };
    const handleRemoveTrade = () => {
        fetch(`http://localhost:5236/api/Trade?tradeID=${tradeID}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };
    const handleMarketTrade = () => {
        const marketTradeData = {
            marketGold: marketGold,
            playerGold: playerGold,
            date: date,
            playerID: playerID,
            marketplaceID: marketplaceID,
            playerItemNames: playerItemNames,
            marketItemNames: marketItemNames
        };

        fetch('http://localhost:5236/api/MarketTrade', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(marketTradeData)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };
    const handleShowMarketTrade = () => {
        fetch(`http://localhost:5236/api/MarketTrade/GetMarketTrades?playerID=${playerID}&marketplaceID=${marketplaceID}`) 
            .then(response => response.json())
            .then(data => setMarketTrade(data));
    };
    return (
        <div>
            <div className='crud-container'>
                <div>
                    <h1 className='i-t'>Make a trade</h1>
                        <div className='input-container'>
                            <input className='create-input' type="number" placeholder="Enter receiver gold" onChange={(e) => setReceiverGold(e.target.value)} />
                            <input className='create-input' type="number" placeholder="Enter requester gold" onChange={(e) => setRequesterGold(e.target.value)} />
                            <input className='create-input' type="text" placeholder="Enter receiver Name" onChange={(e) => setReceiverName(e.target.value)} />
                            <input className='create-input' type="text" placeholder="Enter requester Name" onChange={(e) => setRequesterName(e.target.value)} />
                            <input className='create-input' type="text" placeholder="Enter receiver item names" onChange={(e) => setReceiverItemNames([e.target.value])} />
                            <input className='create-input' type="text" placeholder="Enter requester item names" onChange={(e) => setRequesterItemNames([e.target.value])} />
                            <br/>
                            <button className='violet-bg' onClick={handleCreateTrade}>Make new trade</button>
                        </div>
                </div>
                    <div>
                        <h1 className='i-t'>Finalize trade</h1>
                        <div className='input-container'>
                            <input className='create-input' type="number" placeholder="Enter trade ID" onChange={(e) => setTradeID(e.target.value)} />
                            <input className='create-input' type="number" placeholder="Enter receiver gold" onChange={(e) => setReceiverGold(e.target.value)} />
                            <input className='create-input' type="number" placeholder="Enter requester gold" onChange={(e) => setRequesterGold(e.target.value)} />
                            <br/>
                            <button className='violet-bg' onClick={handleFinalizeTrade}>Finalize trade</button>
                        </div>
                    </div>
                    <div>
                        <h1 className='i-n'>Remove trade</h1>
                        <div className='input-container'>
                            <input className='create-input' type="number" placeholder="Enter trade ID" onChange={(e) => setTradeID(e.target.value)} />
                            <br/>
                            <button className='violet-bg' onClick={handleRemoveTrade}>Remove a trade</button>
                        </div>      
                    </div>  
                
                <div className='market-t'>
                <h1 className='i-n'>Make a trade with a market</h1>
                <div className='input-container'>
                    <input className='create-input' type="number" placeholder="Enter market gold" onChange={(e) => setMarketGold(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter player gold" onChange={(e) => setPlayerGold(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter date" onChange={(e) => setDate(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter player ID" onChange={(e) => setPlayerID(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter marketplace ID" onChange={(e) => setMarketplaceID(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter player item names" onChange={(e) => setPlayerItemNames([e.target.value])} />
                    <input className='create-input' type="text" placeholder="Enter market item names" onChange={(e) => setMarketItemNames([e.target.value])} />
                    <br/>
                    <button className='violet-bg' onClick={handleMarketTrade}>Trade with the market</button>
                </div>
                </div>
                <div className='market-t'>
                <h1 className='i-n'>Trades visualized</h1>
                <div className='input-container'>
                    <input className='create-input' type="number" placeholder="Enter marketplace ID" onChange={(e) => setMarketplaceID(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter player ID" onChange={(e) => setPlayerID(e.target.value)} />
                    <br/>
                    <button className='violet-bg' onClick={handleShowMarketTrade}>Visualize</button>
                    <div className='showroom'>
                        {selectedMarketTrade.map(trade => (
                            <button key={trade.id} 
                            style=
                            {{
                                backgroundColor: '#550d0e',
                                backgroundSize: 'cover',
                                width: '200px',
                                height: '100px', 
                                border: 'none',
                                cursor: 'pointer',
                                margin: '3rem',
                                color: 'white'
                            }}>
                                Id: [{trade.id}]<br/>
                                Date: {trade.date}<br/>
                                Player ID: {trade.playerID}<br/>
                                Marketplace ID: {trade.marketplaceID}<br/>
                                Player gold: {trade.playerGold}<br/>
                                Market gold: {trade.marketGold}<br/>
                                Player items: {trade.playerItemNames}<br/>
                                Market items: {trade.marketItemNames}<br/>
                            </button>
                        ))}
                    </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
export default Trade;
