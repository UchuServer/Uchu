--------------------------------------------------------------

-- L_SHOCK_PANEL_CLIENT.lua

-- Client script for player damaging shock panel.
-- Players, on collision with the panel, will take damage or be smashed instantly
-- based on object config data.
-- Created abeechler... 1/17/11

-------------------------------------------------------------

function onRenderComponentReady(self, msg)  
    self:FireEventServerSide{args = 'renderReady'} 
end