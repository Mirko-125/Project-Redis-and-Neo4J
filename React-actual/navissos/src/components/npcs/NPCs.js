import React, { useEffect, useState } from 'react';
import './NPCs.css';

const NPCs = () => {
    const [npcs, setNPCs] = useState([]);
    const [npcId, setNPCID] = useState('');
    const [npcName, setNPCName] = useState('');
    const [npcAffinity, setNPCAffinity] = useState('');
    const [npcImageURL, setNPCImageURL] = useState('');
    const [npcZone, setNPCZone] = useState('');
    const [npcMood, setNPCMood] = useState('');

    const handleCreateNPC = () => {
        const npcData = {
            name: npcName,
            affinity: npcAffinity,
            imageURL: npcImageURL,
            zone: npcZone,
            mood: npcMood
        };

        fetch('http://localhost:5236/api/NPC', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(npcData)
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
    
    const handleDeleteNPC = () => {
        fetch(`http://localhost:5236/api/NPC?npcId=${npcId}`, {
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

    const handleUpdateNPC = () => {
        const npcData = {
            npcId: npcId,
            name: npcName,
            affinity: npcAffinity,
            imageURL: npcImageURL,
            zone: npcZone,
            mood: npcMood
        };

        fetch('http://localhost:5236/api/NPC/Update', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(npcData)
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
        fetch('http://localhost:5236/api/NPC/GetAll')
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
            <h1 className='i-n'>Admin: Create a new non playable character</h1>
            <div className='create-npc'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter npc name"
                    onChange={(e) => setNPCName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter npc affinity"
                    onChange={(e) => setNPCAffinity(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter npc image url"
                    onChange={(e) => setNPCImageURL(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter npc zone"
                    onChange={(e) => setNPCZone(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter npc mood"
                    onChange={(e) => setNPCMood(e.target.value)}
                />
                <button onClick={handleCreateNPC}>Create a new NPC</button>
            </div>
            <h1 className='i-n'>Admin: Delete a NPC</h1>
            <div className='delete-npc'>
                <input type="number" placeholder="Enter npc ID" onChange={e => setNPCID(e.target.value)} />
                <button onClick={handleDeleteNPC}>Delete a NPC</button>
            </div>
            <h1 className='i-n'>Admin: Update your non playable character</h1>
            <div className='update-npc'>
                <input 
                    className='update-input' 
                    type="number" 
                    placeholder="Enter npc ID" 
                    onChange={e => setNPCID(e.target.value)} 
                />
                <input
                    className='update-input'
                    type="text"
                    placeholder="Enter npc name"
                    onChange={(e) => setNPCName(e.target.value)}
                />
                <input
                    className='update-input'
                    type="text"
                    placeholder="Enter npc affinity"
                    onChange={(e) => setNPCAffinity(e.target.value)}
                />
                <input
                    className='update-input'
                    type="text"
                    placeholder="Enter npc image url"
                    onChange={(e) => setNPCImageURL(e.target.value)}
                />
                <input
                    className='update-input'
                    type="text"
                    placeholder="Enter npc zone"
                    onChange={(e) => setNPCZone(e.target.value)}
                />
                <input
                    className='update-input'
                    type="text"
                    placeholder="Enter npc mood"
                    onChange={(e) => setNPCMood(e.target.value)}
                />
                <button onClick={handleUpdateNPC}>Update NPC</button>
            </div>
        </div>
    );
};

export default NPCs;