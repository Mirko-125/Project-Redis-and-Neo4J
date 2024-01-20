import React, { useEffect, useState } from 'react';
import './Marketplace.css';

function Marketplace() {
    const [marketplace, setMarketplaces] = useState([]);
    
    useEffect(() => {
        fetch('http://localhost:5236/api/Marketplace/GetAllMarketplaces')
            .then(response => response.json())
            .then(data => setMarketplaces(data));
    }, []);
    
    console.log(marketplace);
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
                            {marketplace.properties.zone}
                        </button>
                    ))}
                </div>
        </div>
    );
}

export default Marketplace;
