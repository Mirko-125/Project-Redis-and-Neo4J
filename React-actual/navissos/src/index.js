import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import Create from './components/create-player/Create';
import TitleMenu from './components/title-menu/TitleMenu';
import Ability from './components/ability/Ability';
import Achievement from './components/achievements/Achievement';
import Class from './components/class/Class';
import Item from './components/items/Item';
import Marketplace from './components/marketplace/Marketplace';
import Monster from './components/monster/Monster';
import NPCs from './components/npcs/NPCs';
import Player from './components/player/Player';
import Trade from './components/trade/Trade';
import PlayerProfile from './components/player/PlayerProfile';
import reportWebVitals from './reportWebVitals';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';

/*
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
*/

const router = createBrowserRouter([
  { path: '/', element: <Create></Create> },
  { path: '/title-menu', element: <TitleMenu></TitleMenu> },
  { path: '/ability', element: <Ability></Ability> },
  { path: '/achievement', element: <Achievement></Achievement> },
  { path: '/class', element: <Class></Class> },
  { path: '/item', element: <Item></Item> },
  { path: '/marketplace', element: <Marketplace></Marketplace> },
  { path: '/monster', element: <Monster></Monster> },
  { path: '/npcs', element: <NPCs></NPCs> },
  { path: '/player', element: <Player></Player> },
  { path: '/trade', element: <Trade></Trade> },
  { path: '/player/*', element: <PlayerProfile></PlayerProfile>}
]);


ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <RouterProvider router={router}/>
  </React.StrictMode>
)

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
