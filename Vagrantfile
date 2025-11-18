# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  # https://docs.vagrantup.com.
  # boxes at https://vagrantcloud.com/search.
  config.vm.box = "cloud-image/ubuntu-24.04"
  config.vm.box_version = "20251031.0.0"
  config.vm.box_check_update = false

  # config.vm.network "forwarded_port", guest: 80, host: 80
  # config.vm.network "forwarded_port", guest: 443, host: 443

  # config.vm.synced_folder "../data", "/vagrant_data"

  config.vm.synced_folder ".", "/vagrant", disabled: true

  config.vm.provider "virtualbox" do |vb|
    vb.gui = false
    vb.memory = "4096"
    vb.cpus = 4
  end
  
  # config.vm.provision "file", source: "", destination: ""

  config.vm.provision "shell", inline: <<-SHELL
    sudo -i
    apt-get update
  SHELL
end
