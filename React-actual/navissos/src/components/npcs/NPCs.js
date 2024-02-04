import React, { useEffect, useState } from 'react';
import './NPCs.css';
import '../../styling/CrudContainer.css'

const NPCs = () => {
    const [npcs, setNPCs] = useState([]);
    const [npcName, setNPCName] = useState('');
    const [npcAffinity, setNPCAffinity] = useState('');
    const [npcImageURL, setNPCImageURL] = useState('');
    const [npcZone, setNPCZone] = useState('');
    const [npcMood, setNPCMood] = useState('');
    const [singleNPC, setSingleNPC] = useState([]);

    const handleFindNPC = () => {
        return fetch(`http://localhost:5236/api/NPC/GetOne?npcName=${npcName}`)
            .then(response => response.json())
            .then(data => {
                setSingleNPC(data);
                return data;
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

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
                window.location.reload();
                return data;
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };
    
    const handleDeleteNPC = () => {
        fetch(`http://localhost:5236/api/NPC?npcName=${npcName}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
                return data;
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleUpdateNPC = () => {
        const npcData = {
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
                window.location.reload();
                return data;
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
        <div className='wrap-n'>
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
            <div className='crud-container'>
                <div>
                    <h1 className='i-n'>Admin: Create a new non playable character</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Enter npc name"
                            onChange={(e) => setNPCName(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc affinity"
                            onChange={(e) => setNPCAffinity(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc image url"
                            onChange={(e) => setNPCImageURL(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc zone"
                            onChange={(e) => setNPCZone(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc mood"
                            onChange={(e) => setNPCMood(e.target.value)}
                        />
                        <button className='green-bg' onClick={handleCreateNPC}>Create a new NPC</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-n'>Admin: Update your non playable character</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Enter npc name"
                            onChange={(e) => setNPCName(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc affinity"
                            onChange={(e) => setNPCAffinity(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc image url"
                            onChange={(e) => setNPCImageURL(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc zone"
                            onChange={(e) => setNPCZone(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter npc mood"
                            onChange={(e) => setNPCMood(e.target.value)}
                        />
                        <button className='cream-bg' onClick={handleUpdateNPC}>Update NPC</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-n'>Admin: Delete a NPC</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="Enter npc's name" onChange={e => setNPCName(e.target.value)} />
                        <button className='red-bg' onClick={handleDeleteNPC}>Delete a NPC</button>
                    </div>
                    <h1 className='i-p'>Gamewise: See a the certain NPC</h1>
                    <div className='input-container'>
                        <input type="text" placeholder="NPC's name is..." onChange={e => setNPCName(e.target.value)} />
                        <button className='green-bg' onClick={handleFindNPC}>Call</button>
                        <div className='players'>
                            <div style={{
                                backgroundColor: 'black',
                                backgroundSize: 'cover',
                                width: '200px',
                                height: '200px',
                                border: 'none',
                                cursor: 'pointer',
                                margin: '3rem',
                                color: 'white'
                            }}
                            >
                                Name: {singleNPC.name}<br/>
                                Affinity: {singleNPC.affinity}<br/>
                                Image source: {singleNPC.imageURL}<br/>
                                Zone: {singleNPC.zone}<br/>
                                Mood: {singleNPC.mood}<br/>
                            </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    );
};

export default NPCs;