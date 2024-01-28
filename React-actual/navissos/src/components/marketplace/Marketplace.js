import React, { useEffect, useState } from 'react';
import './Marketplace.css';

function Marketplace() {
    const [marketplace, setMarketplaces] = useState([]);
    const [zone, setZone] = useState('');
    const [itemCount, setItemCount] = useState(0);
    const [restockCycle, setRestockCycle] = useState(0);
    const [itemName, setItemName] = useState('');
    const [marketZone, setMarketZone] = useState(0); 
    const [marketplaceID, setMarketplaceID] = useState(0);
    
    useEffect(() => {
        fetch(`http://localhost:5236/api/Marketplace/GetAll`)
            .then(response => response.json())
            .then(data => setMarketplaces(data));
    }, []);
    
    const handleCreateMarketplace = () => {
        const marketplaceData = {
            zone: zone,
            itemCount: itemCount,
            restockCycle: restockCycle
        };

        fetch(`http://localhost:5236/api/Marketplace`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(marketplaceData)
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

    const handleUpdateMarketplace = () => {
        const marketplaceData = {
            markketplaceID: marketplaceID,
            zone: zone,
            restockCycle: restockCycle
        };

        fetch(`http://localhost:5236/api/Marketplace/Update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(marketplaceData)
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

    const handleDeleteMarketplace = () => {
        fetch(`http://localhost:5236/api/Marketplace?zone=${zone}`, {
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

    const handleAddItemToMarketplace = () => {

        fetch(`http://localhost:5236/api/Marketplace/AddItem?zoneName=${marketZone}&itemName=${itemName}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
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

    return (
        <div>
            <h1 className='i-mp'>Markets</h1>
                <div className="markets">
                    {marketplace.map(marketplace => (
                        <button key={marketplace.id} 
                        style={{
                            backgroundSize: 'cover',
                            width: '200px', 
                            height: '100px', 
                            border: 'none',
                            cursor: 'pointer',
                            margin: '3rem'
                        }}
                        >
                            {marketplace.zone}
                        </button>
                    ))}
                </div>
            <h1 className='i-a'>Admin: Add a new Marketplace</h1>
            <div className='create-marketplace'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter zone"
                    onChange={(e) => setZone(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter item count"
                    onChange={(e) => setItemCount(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter restock cycle"
                    onChange={(e) => setRestockCycle(parseInt(e.target.value))}
                />
                <button onClick={handleCreateMarketplace}>Create Marketplace</button>
            </div>
            <h1 className='i-a'>Admin: Move a market</h1>
            <div className='create-marketplace'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter zone"
                    onChange={(e) => setZone(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter market's original ID"
                    onChange={(e) => setMarketplaceID(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter restock cycle"
                    onChange={(e) => setRestockCycle(parseInt(e.target.value))}
                />
                <button onClick={handleUpdateMarketplace}>Update marketplace</button>
            </div>
            <h1 className='i-a'>Add an item to the selected marketplace</h1>
            <div className='create-marketplace'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter item name"
                    onChange={(e) => setItemName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter market's zone"
                    onChange={(e) => setMarketZone(parseInt(e.target.value))}
                />
                <button onClick={handleAddItemToMarketplace}>Add item</button>
            </div>
            <h1 className='i-a'>Delete marketplaces in a certain zone</h1>
            <div className='create-marketplace'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter zone"
                    onChange={(e) => setZone(e.target.value)}
                />
                <button onClick={handleDeleteMarketplace}>Delete marketplace</button>
            </div>                
        </div>
    );
}

export default Marketplace;
