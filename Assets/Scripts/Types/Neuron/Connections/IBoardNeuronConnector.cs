﻿using System.Threading;
using System.Threading.Tasks;
using Types.Neuron.Runtime;

namespace Types.Neuron.Connections {
    public interface IBoardNeuronConnector {
        
        SemaphoreSlim ConnectionLock { get; }
        
        Task Connect(IBoardNeuron n1, IBoardNeuron n2, int delay = 0);
        Task Disconnect(IBoardNeuron n1, IBoardNeuron n2, int delay = 0);
    }
}