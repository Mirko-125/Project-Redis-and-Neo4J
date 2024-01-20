import React, { useEffect, useState } from 'react';
import './NPCs.css';

const NPCs = () => {
    const [npcs, setNPCs] = useState([]);
    const [npcId, setNPCID] = useState('');

    const handleDeleteNPC = () => {
        fetch(`http://localhost:5236/api/NPC/DeleteNPC?npcId=${npcId}`, {
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

    useEffect(() => {
        fetch('http://localhost:5236/api/NPC/GetAllNPCs') // Replace '/api/npcs' with your actual API endpoint
            .then(response => response.json())
            .then(data => setNPCs(data));
    }, []);
    return (
        <div>
            <h1 className='i-n'>NPCs</h1>
            <div className='people'>
            {npcs.map(npc => (
                <button key={npc.id} style={{
                    backgroundImage: `url(${(npc.imageURL)})`,
                    backgroundSize: 'cover',
                    width: '200px',
                    height: '100px', 
                    border: 'none',
                    cursor: 'pointer',
                    margin: '3rem'
                }}>
                    {npc.name}
                </button>
            ))}
            </div>
            <h1 className='i-n'>NPCs</h1>
            <div className='delete-npc'>
                <input type="number" placeholder="Enter npc ID" value={npcId} onChange={e => setNPCID(e.target.value)} />
                <button onClick={handleDeleteNPC}>Delete a NPC</button>
            </div>
        </div>
    );
};

export default NPCs;