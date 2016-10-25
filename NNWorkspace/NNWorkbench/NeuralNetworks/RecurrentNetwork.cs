using NNWorkbench.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNWorkbench.NeuralNetworks
{
    public class RecurrentNetwork
    {
        private Layer[] layers;
        public readonly NetworkConfiguration Configuration;

        /// <summary>
        /// Creates a Recurrent Neural Network with the specified number of layers and one output Neuron.
        /// </summary>
        /// <param name="layerSize"></param>
        public RecurrentNetwork(NetworkConfiguration configuration, params int[] layerSize)
        {
            this.Configuration = configuration;
            configuration.InitialiseRandom();

            layers = new Layer[layerSize.Length - 1];
            for (int i = 0; i < layers.Length - 1; i++)
            {
                layers[i] = new LSTMLayer(configuration, layerSize[i + 1], layerSize[i]);
            }
            layers[layers.Length - 1] = new SoftmaxLayer(configuration, layerSize[layerSize.Length - 1], layerSize[layerSize.Length - 2]);
        }

        public IList<Layer> Layers
        {
            get { return layers; }
        }

        public double MeanAbsoluteWeight
        {
            get
            {
                return layers.Sum(l => l.SumAbsoluteWeight) / layers.Sum(l => l.WeightCount);
            }
        }

        public void Save(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                Save(stream);
            }
        }

        public void Save(Stream output)
        {
            using (BinaryWriter writer = new BinaryWriter(output, Encoding.UTF8, true))
            {
                foreach (Layer layer in layers)
                {
                    layer.Save(writer);
                }
                writer.Flush();
            }
        }

        public void Load(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                Load(stream);
            }
        }

        public void Load(Stream input)
        {
            using (BinaryReader reader = new BinaryReader(input, Encoding.UTF8, true))
            {
                foreach (Layer layer in layers)
                {
                    layer.Load(reader);
                }
            }
        }

        public void Export(StringBuilder builder)
        {
            foreach (Layer layer in layers)
            {
                layer.Export(builder);
            }
        }
    }
}
