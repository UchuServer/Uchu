local function shift(fn)
	coroutine.yield(fn)
end

local function proc(fn)
	local coro = coroutine.wrap(fn)
	local cont = function (...)
		local cb = coro(unpack(arg))
		if cb then cb() end
	end
	cont(cont)
end

Delay = {
	proc = proc,
	shift = shift,
}
