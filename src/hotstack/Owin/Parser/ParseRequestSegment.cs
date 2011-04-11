using System;
using hotstack.Owin.Impl;

namespace hotstack.Owin.Parser
{
    public delegate void ParseRequestSegment( Request request, ConsumableSegment<byte> segment);
}